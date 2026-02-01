from fastapi import APIRouter, HTTPException, Request, Depends
from pydantic import BaseModel, Field, validator
from transformers import pipeline
from slowapi import Limiter
from slowapi.util import get_remote_address
from config import settings
from dependencies import verify_api_key
import logging
import hashlib
import time

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

# Model cache to prevent reloading
_model_cache = {}
_model_load_lock = False

def get_classifier():
    """Get or create classifier model instance"""
    global _model_cache, _model_load_lock
    
    if 'classifier' in _model_cache:
        return _model_cache['classifier']
    
    if _model_load_lock:
        raise HTTPException(status_code=503, detail="Model is currently loading")
    
    try:
        _model_load_lock = True
        classifier = pipeline(
            'zero-shot-classification',
            model=settings.CLASSIFIER_MODEL,
            device='cpu',
            model_kwargs={'cache_dir': settings.MODEL_CACHE_DIR}
        )
        # Warm up
        classifier('initialization', ['test', 'warmup'])
        _model_cache['classifier'] = classifier
        logger.info(f"Classifier model loaded: {settings.CLASSIFIER_MODEL}")
        return classifier
    except Exception as e:
        logger.error(f"Failed to load classifier model: {e}")
        raise HTTPException(status_code=503, detail="Classifier model unavailable")
    finally:
        _model_load_lock = False

class ClassifyRequest(BaseModel):
    text: str = Field(
        ..., 
        min_length=1, 
        max_length=settings.MAX_TEXT_LENGTH,
        description="Text to classify"
    )
    candidate_labels: list[str] = Field(
        ..., 
        min_items=settings.MIN_CANDIDATE_LABELS,
        max_items=settings.MAX_CANDIDATE_LABELS,
        description="Possible classification labels"
    )
    multi_label: bool = Field(
        default=False,
        description="Whether to allow multiple labels (independent probabilities)"
    )
    
    @validator('candidate_labels')
    def validate_labels(cls, v):
        """Ensure labels are unique and non-empty"""
        if len(v) != len(set(v)):
            raise ValueError("Candidate labels must be unique")
        if any(not label.strip() for label in v):
            raise ValueError("Candidate labels cannot be empty or whitespace")
        return [label.strip() for label in v]
    
    @validator('text')
    def validate_text(cls, v):
        """Sanitize text input"""
        stripped = v.strip()
        if not stripped:
            raise ValueError("Text cannot be empty or whitespace only")
        return stripped
    
    class Config:
        json_schema_extra = {
            "example": {
                "text": "I love this product! It works amazingly well.",
                "candidate_labels": ["positive", "negative", "neutral"],
                "multi_label": False
            }
        }

class ClassifyResponse(BaseModel):
    predicted_label: str
    scores: dict[str, float]
    multi_label: bool
    processing_time_ms: float

class BatchClassifyRequest(BaseModel):
    items: list[ClassifyRequest] = Field(
        ...,
        min_items=1,
        max_items=10,
        description="Batch of texts to classify"
    )

class BatchClassifyResponse(BaseModel):
    results: list[ClassifyResponse]
    total_processing_time_ms: float

@router.post("/classify", response_model=ClassifyResponse)
@limiter.limit(settings.RATE_LIMIT_CLASSIFY)
async def classify_text(
    request: Request, 
    item: ClassifyRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Classify text using zero-shot classification.
    
    - **text**: The text to classify (1-5000 characters)
    - **candidate_labels**: List of possible labels (2-20 labels)
    - **multi_label**: Whether multiple labels can apply simultaneously
    
    Returns the most likely label and confidence scores for all labels.
    """
    start_time = time.time()
    
    try:
        classifier = get_classifier()
        
        result = classifier(
            item.text, 
            item.candidate_labels,
            multi_label=item.multi_label
        )
        
        scores = {
            label: round(float(score), 4) 
            for label, score in zip(result['labels'], result['scores'])
        }
        
        processing_time = (time.time() - start_time) * 1000
        
        return ClassifyResponse(
            predicted_label=result['labels'][0],
            scores=scores,
            multi_label=item.multi_label,
            processing_time_ms=round(processing_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Classification error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500, 
            detail="Classification processing failed"
        )

@router.post("/classify/batch", response_model=BatchClassifyResponse)
@limiter.limit("5/minute")
async def classify_batch(
    request: Request,
    batch_request: BatchClassifyRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Classify multiple texts in a single request.
    
    Limited to 10 items per batch for performance reasons.
    """
    start_time = time.time()
    results = []
    
    try:
        classifier = get_classifier()
        
        for item in batch_request.items:
            item_start = time.time()
            
            result = classifier(
                item.text,
                item.candidate_labels,
                multi_label=item.multi_label
            )
            
            scores = {
                label: round(float(score), 4)
                for label, score in zip(result['labels'], result['scores'])
            }
            
            processing_time = (time.time() - item_start) * 1000
            
            results.append(ClassifyResponse(
                predicted_label=result['labels'][0],
                scores=scores,
                multi_label=item.multi_label,
                processing_time_ms=round(processing_time, 2)
            ))
        
        total_time = (time.time() - start_time) * 1000
        
        return BatchClassifyResponse(
            results=results,
            total_processing_time_ms=round(total_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Batch classification error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Batch classification processing failed"
        )
        