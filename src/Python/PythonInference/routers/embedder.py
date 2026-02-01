from fastapi import APIRouter, HTTPException, Request, Depends
from pydantic import BaseModel, Field, validator
from sentence_transformers import SentenceTransformer
from slowapi import Limiter
from slowapi.util import get_remote_address
from config import settings
from dependencies import verify_api_key
import logging
import numpy as np
import time

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

_model_cache = {}
_model_load_lock = False

def get_embedder():
    """Get or create embedding model instance"""
    global _model_cache, _model_load_lock
    
    if 'embedder' in _model_cache:
        return _model_cache['embedder']
    
    if _model_load_lock:
        raise HTTPException(status_code=503, detail="Model is currently loading")
    
    try:
        _model_load_lock = True
        model = SentenceTransformer(
            settings.EMBEDDING_MODEL,
            cache_folder=settings.MODEL_CACHE_DIR
        )
        _model_cache['embedder'] = model
        logger.info(f"Embedding model loaded: {settings.EMBEDDING_MODEL}")
        return model
    except Exception as e:
        logger.error(f"Failed to load embedding model: {e}")
        raise HTTPException(status_code=503, detail="Embedding model unavailable")
    finally:
        _model_load_lock = False

class EmbedRequest(BaseModel):
    sentences: list[str] = Field(
        ..., 
        min_items=1, 
        max_items=settings.MAX_SENTENCES_EMBED,
        description="Sentences to embed"
    )
    normalize: bool = Field(
        default=True,
        description="Normalize embeddings to unit vectors"
    )
    
    @validator('sentences')
    def validate_sentences(cls, v):
        """Validate and sanitize sentences"""
        cleaned = []
        for sentence in v:
            stripped = sentence.strip()
            if not stripped:
                raise ValueError("Sentences cannot be empty or whitespace only")
            if len(stripped) > settings.MAX_TEXT_LENGTH:
                raise ValueError(f"Sentence exceeds maximum length of {settings.MAX_TEXT_LENGTH}")
            cleaned.append(stripped)
        return cleaned
    
    class Config:
        json_schema_extra = {
            "example": {
                "sentences": [
                    "The cat sits on the mat.",
                    "A feline rests on a rug."
                ],
                "normalize": True
            }
        }

class EmbedResponse(BaseModel):
    embeddings: list[list[float]]
    dimension: int
    count: int
    normalized: bool
    processing_time_ms: float

class SentencePairRequest(BaseModel):
    sentence1: str = Field(..., min_length=1, max_length=settings.MAX_TEXT_LENGTH)
    sentence2: str = Field(..., min_length=1, max_length=settings.MAX_TEXT_LENGTH)
    
    @validator('sentence1', 'sentence2')
    def validate_sentence(cls, v):
        stripped = v.strip()
        if not stripped:
            raise ValueError("Sentence cannot be empty or whitespace only")
        return stripped

class SimilarityResponse(BaseModel):
    similarity: float
    processing_time_ms: float

@router.post("/embed", response_model=EmbedResponse)
@limiter.limit(settings.RATE_LIMIT_EMBED)
async def embed_sentences(
    request: Request,
    req: EmbedRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Generate sentence embeddings using transformer models.
    
    - **sentences**: List of sentences to embed (1-100 sentences)
    - **normalize**: Whether to normalize embeddings to unit vectors
    
    Returns dense vector representations suitable for semantic similarity.
    """
    start_time = time.time()
    
    try:
        model = get_embedder()
        
        embeddings = model.encode(
            req.sentences,
            normalize_embeddings=req.normalize,
            batch_size=settings.BATCH_SIZE,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        embeddings_list = embeddings.tolist()
        processing_time = (time.time() - start_time) * 1000
        
        return EmbedResponse(
            embeddings=embeddings_list,
            dimension=len(embeddings_list[0]) if embeddings_list else 0,
            count=len(embeddings_list),
            normalized=req.normalize,
            processing_time_ms=round(processing_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Embedding error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Embedding generation failed"
        )

@router.post("/embed/mean", response_model=dict)
@limiter.limit(settings.RATE_LIMIT_EMBED)
async def embed_mean(
    request: Request,
    req: EmbedRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Generate a single mean embedding from multiple sentences.
    
    Useful for creating document-level representations.
    """
    start_time = time.time()
    
    try:
        model = get_embedder()
        
        embeddings = model.encode(
            req.sentences,
            normalize_embeddings=False,
            batch_size=settings.BATCH_SIZE,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        mean_embedding = np.mean(embeddings, axis=0)
        
        if req.normalize:
            mean_embedding = mean_embedding / np.linalg.norm(mean_embedding)
        
        processing_time = (time.time() - start_time) * 1000
        
        return {
            "embedding": mean_embedding.tolist(),
            "dimension": len(mean_embedding),
            "source_count": len(req.sentences),
            "normalized": req.normalize,
            "processing_time_ms": round(processing_time, 2)
        }
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Mean embedding error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Mean embedding generation failed"
        )