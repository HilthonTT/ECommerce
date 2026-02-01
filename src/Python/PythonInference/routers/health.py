from fastapi import APIRouter, Request
from slowapi import Limiter
from slowapi.util import get_remote_address
import logging
import psutil
import time
from datetime import datetime

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

# Track API start time
_start_time = time.time()

@router.get("/health")
@limiter.limit("200/minute")
async def health_check(request: Request):
    """
    Basic health check endpoint.
    
    Returns the operational status of the API.
    """
    return {
        "status": "healthy",
        "timestamp": datetime.utcnow().isoformat()
    }
    
@router.get("/health/detailed")
@limiter.limit("60/minute")
async def detailed_health_check(request: Request):
    """
    Detailed health check with system metrics.
    
    Provides information about:
    - Model availability
    - System resources (CPU, memory)
    - Uptime
    """
    try:
        # Check model availability
        models_status = {}
        
        try:
            from routers.classifier import get_classifier
            classifier = get_classifier()
            models_status['classifier'] = 'loaded' if classifier else 'unavailable'
        except Exception as e:
            models_status['classifier'] = f'error: {str(e)[:50]}'
        
        try:
            from routers.embedder import get_embedder
            embedder = get_embedder()
            models_status['embedder'] = 'loaded' if embedder else 'unavailable'
        except Exception as e:
            models_status['embedder'] = f'error: {str(e)[:50]}'
        
        # System metrics
        cpu_percent = psutil.cpu_percent(interval=0.1)
        memory = psutil.virtual_memory()
        disk = psutil.disk_usage('/')
        
        uptime_seconds = time.time() - _start_time
        
        return {
            "status": "healthy",
            "timestamp": datetime.utcnow().isoformat(),
            "uptime_seconds": round(uptime_seconds, 2),
            "models": models_status,
            "system": {
                "cpu_percent": cpu_percent,
                "memory_percent": memory.percent,
                "memory_available_mb": round(memory.available / (1024 * 1024), 2),
                "disk_percent": disk.percent,
                "disk_free_gb": round(disk.free / (1024 ** 3), 2)
            }
        }
    except Exception as e:
        logger.error(f"Health check error: {e}", exc_info=True)
        return {
            "status": "degraded",
            "timestamp": datetime.utcnow().isoformat(),
            "error": str(e)
        }
    
@router.get("/health/ready")
@limiter.limit("200/minute")
async def readiness_check(request: Request):
    """
    Kubernetes-style readiness probe.
    
    Returns 200 if the service is ready to accept traffic.
    """
    try:
        from routers.classifier import get_classifier
        from routers.embedder import get_embedder
        
        # Verify models can be accessed
        classifier = get_classifier()
        embedder = get_embedder()
        
        if classifier and embedder:
            return {"ready": True}
        else:
            return {"ready": False, "reason": "Models not loaded"}, 503
    except Exception as e:
        logger.error(f"Readiness check failed: {e}")
        return {"ready": False, "reason": str(e)}, 503

@router.get("/health/live")
@limiter.limit("200/minute")
async def liveness_check(request: Request):
    """
    Kubernetes-style liveness probe.
    
    Returns 200 if the service is alive.
    """
    return {"alive": True}
