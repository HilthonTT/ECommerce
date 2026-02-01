import logging
import time
from fastapi import FastAPI, Request, status
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse
from slowapi import Limiter, _rate_limit_exceeded_handler
from slowapi.util import get_remote_address
from slowapi.errors import RateLimitExceeded
from fastapi.middleware.trustedhost import TrustedHostMiddleware
from fastapi.middleware.gzip import GZipMiddleware
from routers import classifier, embedder, health, similarity
from config import settings

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

limiter = Limiter(key_func=get_remote_address)

async def lifespan(app: FastAPI):
    """Application lifespan events"""
    logger.info("Starting ML Inference API")
    # Startup: models are loaded in routers
    yield
    # Shutdown
    logger.info("Shutting down ML Inference API")

app = FastAPI(
    title="ML Inference API",
    description="Zero-shot classification and sentence embedding endpoints",
    version="1.0.0",
    docs_url="/docs" if settings.ENABLE_DOCS else None,
    redoc_url="/redoc" if settings.ENABLE_DOCS else None,
    lifespan=lifespan,
)

app.state.limiter = limiter
app.add_exception_handler(RateLimitExceeded, _rate_limit_exceeded_handler)

if settings.ALLOWED_HOSTS:
    app.add_middleware(
        TrustedHostMiddleware, 
        allowed_hosts=settings.ALLOWED_HOSTS.split(",")
    )

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS.split(",") if settings.CORS_ORIGINS != "*" else ["*"],
    allow_credentials=True,
    allow_methods=["GET", "POST"],
    allow_headers=["*"],
    max_age=3600,
)

app.add_middleware(GZipMiddleware, minimum_size=1_000)

@app.middleware("http")
async def log_requests(request: Request, call_next):
    """Log all requests with timing"""
    start_time = time.time()
    
    response = await call_next(request)
    process_time = time.time() - start_time
    logger.info(
        f"{request.method} {request.url.path} "
        f"status={response.status_code} duration={process_time:.3f}s"
    )
    
    response.headers["X-Process-Time"] = str(process_time)
    return response
    
@app.middleware("http")
async def add_security_headers(request: Request, call_next):
    """Add security headers to all responses"""
    response = await call_next(request)
    response.headers["X-Content-Type-Options"] = "nosniff"
    response.headers["X-Frame-Options"] = "DENY"
    response.headers["X-XSS-Protection"] = "1; mode=block"
    response.headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains"
    return response

@app.exception_handler(Exception)
async def global_exception_handler(request: Request, exc: Exception):
    """Handle unexpected exceptions"""
    logger.error(f"Unhandled exception: {exc}", exc_info=True)
    return JSONResponse(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        content={
            "detail": "Internal server error" if not settings.DEBUG else str(exc)
        }
    )

app.include_router(health.router, tags=["health"])
app.include_router(classifier.router, prefix="/api/v1", tags=["classification"])
app.include_router(embedder.router, prefix="/api/v1", tags=["embeddings"])
app.include_router(similarity.router, prefix="/api/v1", tags=["similarity"])

@app.get("/", include_in_schema=False)
@limiter.limit("100/minute")
async def root(request: Request):
    """API root endpoint"""
    return {
        "name": "ML Inference API",
        "version": "2.0.0",
        "status": "operational",
        "docs": "/docs" if settings.ENABLE_DOCS else "disabled",
        "endpoints": {
            "health": "/health",
            "classification": "/api/v1/classify",
            "embeddings": "/api/v1/embed",
            "similarity": "/api/v1/similarity"
        }
    }

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(
        "main:app",
        host=settings.HOST,
        port=settings.PORT,
        reload=settings.DEBUG,
        log_level=settings.LOG_LEVEL.lower()
    )