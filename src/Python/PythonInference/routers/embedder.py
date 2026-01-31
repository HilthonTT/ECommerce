from fastapi import APIRouter, HTTPException, Request
from pydantic import BaseModel, Field
from sentence_transformers import SentenceTransformer
from slowapi import Limiter
from slowapi.util import get_remote_address
import logging

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

try:
    model = SentenceTransformer('sentence-transformers/all-MiniLM-L6-v2')
    logger.info("Embedding model loaded successfully")
except Exception as e:
    logger.error(f"Failed to load embedding model: {e}")
    model = None

class EmbedRequest(BaseModel):
    sentences: list[str] = Field(..., min_items=1, max_items=100, description="Sentences to embed")
    
    class Config:
        json_schema_extra = {
            "example": {
                "sentences": ["This is a sentence.", "This is another sentence."]
            }
        }

class EmbedResponse(BaseModel):
    embeddings: list[list[float]]
    dimension: int
    count: int
    
class EmbedRequest(BaseModel):
    sentences: list[str]

@router.post("/embed", response_model=EmbedResponse)
@limiter.limit("20/minute")
async def embed_sentences(request: Request,req: EmbedRequest) -> list[list[float]]:
    """
    Generate sentence embeddings.
    
    Returns a list of embedding vectors, one per input sentence.
    """
    if model is None:
        raise HTTPException(status_code=503, detail="Embedding model not available")

    try:
        embeddings = model.encode(req.sentences)
        embeddings_list = embeddings.tolist()
        
        return EmbedResponse(
            embeddings=embeddings_list,
            dimension=len(embeddings_list[0]) if embeddings_list else 0,
            count=len(embeddings_list)
        )
    except Exception as e:
        logger.error(f"Embedding error: {e}")
        raise HTTPException(status_code=500, detail=f"Embedding failed: {str(e)}")
