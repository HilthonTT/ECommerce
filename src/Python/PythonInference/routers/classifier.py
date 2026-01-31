from fastapi import APIRouter, HTTPException, Request
from pydantic import BaseModel, Field
from transformers import pipeline
from slowapi import Limiter
from slowapi.util import get_remote_address
import logging

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

try:
    classifier = pipeline(
        'zero-shot-classification',
        model='cross-encoder/nli-MiniLM2-L6-H768',
        device='cpu'
    )
    # Warm up the model
    classifier('warm up', ['a', 'b'])
    logger.info("Classifier model loaded successfully")
except Exception as e:
    logger.error(f"Failed to load classifier model: {e}")
    classifier = None

class ClassifyRequest(BaseModel):
    text: str = Field(..., min_length=1, max_length=5000, description="Text to classify")
    candidate_labels: list[str] = Field(..., min_items=2, description="Possible labels")
    
    class Config:
        json_schema_extra = {
            "example": {
                "text": "I love this product!",
                "candidate_labels": ["positive", "negative", "neutral"]
            }
        }

class ClassifyResponse(BaseModel):
    predicted_label: str 
    scores: dict[str, float]

@router.post("/classify", response_model=ClassifyResponse)
@limiter.limit("10/minute")
async def classify_text(request: Request, item: ClassifyRequest):
    """
    Classify text using zero-shot classification.
    
    Returns the most likely label and confidence scores for all labels.
    """
    if classifier is None:
        raise HTTPException(status_code=503, detail="Classifier model not available")
    
    try:
        result = classifier(item.text, item.candidate_labels)
        
        scores = {label: float(score) for label, score in zip(result['labels'], result['scores'])}
        
        return ClassifyResponse(
            predicted_label=result['labels'][0], 
            scores=scores
        )
    except Exception as e:
        logger.error(f"Classification error: {e}")
        raise HTTPException(status_code=500, detail=f"Classification failed: {str(e)}")
    