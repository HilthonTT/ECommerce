from fastapi import APIRouter, HTTPException, Request, Depends
from pydantic import BaseModel, Field, validator
from sentence_transformers import SentenceTransformer, util
from slowapi import Limiter
from slowapi.util import get_remote_address
from config import settings
from dependencies import verify_api_key
import logging
import time
import numpy as np

logger = logging.getLogger(__name__)
router = APIRouter()
limiter = Limiter(key_func=get_remote_address)

def get_embedder():
    """Import embedder from embedder module"""
    from routers.embedder import get_embedder as _get_embedder
    return _get_embedder()

class SimilarityPairRequest(BaseModel):
    sentence1: str = Field(..., min_length=1, max_length=settings.MAX_TEXT_LENGTH)
    sentence2: str = Field(..., min_length=1, max_length=settings.MAX_TEXT_LENGTH)
    
    @validator('sentence1', 'sentence2')
    def validate_sentence(cls, v):
        stripped = v.strip()
        if not stripped:
            raise ValueError("Sentence cannot be empty or whitespace only")
        return stripped
    
    class Config:
        json_schema_extra = {
            "example": {
                "sentence1": "The cat sits on the mat.",
                "sentence2": "A feline rests on a rug."
            }
        }

class SimilarityResponse(BaseModel):
    similarity: float
    interpretation: str
    processing_time_ms: float

class RankRequest(BaseModel):
    query: str = Field(..., min_length=1, max_length=settings.MAX_TEXT_LENGTH)
    candidates: list[str] = Field(..., min_items=1, max_items=100)
    top_k: int = Field(default=5, ge=1, le=100)
    
    @validator('query')
    def validate_query(cls, v):
        stripped = v.strip()
        if not stripped:
            raise ValueError("Query cannot be empty")
        return stripped
    
    @validator('candidates')
    def validate_candidates(cls, v):
        cleaned = []
        for candidate in v:
            stripped = candidate.strip()
            if not stripped:
                raise ValueError("Candidates cannot be empty")
            cleaned.append(stripped)
        return cleaned
    
    class Config:
        json_schema_extra = {
            "example": {
                "query": "machine learning algorithms",
                "candidates": [
                    "neural networks and deep learning",
                    "cooking recipes for pasta",
                    "supervised learning techniques",
                    "gardening tips for beginners"
                ],
                "top_k": 3
            }
        }

class RankedResult(BaseModel):
    text: str
    score: float
    rank: int

class RankResponse(BaseModel):
    query: str
    results: list[RankedResult]
    processing_time_ms: float

class ClusterRequest(BaseModel):
    sentences: list[str] = Field(..., min_items=2, max_items=100)
    threshold: float = Field(default=0.75, ge=0.0, le=1.0)
    
    @validator('sentences')
    def validate_sentences(cls, v):
        cleaned = []
        for sentence in v:
            stripped = sentence.strip()
            if not stripped:
                raise ValueError("Sentences cannot be empty")
            cleaned.append(stripped)
        return cleaned

class Cluster(BaseModel):
    centroid_text: str
    members: list[str]
    avg_similarity: float

class ClusterResponse(BaseModel):
    clusters: list[Cluster]
    num_clusters: int
    processing_time_ms: float

@router.post("/similarity", response_model=SimilarityResponse)
@limiter.limit(settings.RATE_LIMIT_SIMILARITY)
async def compute_similarity(
    request: Request,
    req: SimilarityPairRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Compute semantic similarity between two sentences.
    
    Returns a cosine similarity score between -1 and 1, where:
    - 1.0 = identical meaning
    - 0.0 = unrelated
    - -1.0 = opposite meaning (rare)
    """
    start_time = time.time()
    
    try:
        model = get_embedder()
        
        embeddings = model.encode(
            [req.sentence1, req.sentence2],
            normalize_embeddings=True,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        similarity = float(util.cos_sim(embeddings[0], embeddings[1])[0][0])
        
        # Interpret similarity score
        if similarity >= 0.9:
            interpretation = "Very high similarity - nearly identical meaning"
        elif similarity >= 0.7:
            interpretation = "High similarity - closely related"
        elif similarity >= 0.5:
            interpretation = "Moderate similarity - somewhat related"
        elif similarity >= 0.3:
            interpretation = "Low similarity - loosely related"
        else:
            interpretation = "Very low similarity - mostly unrelated"
        
        processing_time = (time.time() - start_time) * 1000
        
        return SimilarityResponse(
            similarity=round(similarity, 4),
            interpretation=interpretation,
            processing_time_ms=round(processing_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Similarity computation error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Similarity computation failed"
        )

@router.post("/similarity/rank", response_model=RankResponse)
@limiter.limit(settings.RATE_LIMIT_SIMILARITY)
async def rank_by_similarity(
    request: Request,
    req: RankRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Rank candidates by semantic similarity to a query.
    
    Useful for semantic search, finding most relevant documents, etc.
    """
    start_time = time.time()
    
    try:
        model = get_embedder()
        
        # Encode query and candidates
        query_embedding = model.encode(
            req.query,
            normalize_embeddings=True,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        candidate_embeddings = model.encode(
            req.candidates,
            normalize_embeddings=True,
            batch_size=settings.BATCH_SIZE,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        # Compute similarities
        similarities = util.cos_sim(query_embedding, candidate_embeddings)[0]
        
        # Get top k results
        top_k = min(req.top_k, len(req.candidates))
        top_indices = np.argsort(similarities)[::-1][:top_k]
        
        results = [
            RankedResult(
                text=req.candidates[idx],
                score=round(float(similarities[idx]), 4),
                rank=rank + 1
            )
            for rank, idx in enumerate(top_indices)
        ]
        
        processing_time = (time.time() - start_time) * 1000
        
        return RankResponse(
            query=req.query,
            results=results,
            processing_time_ms=round(processing_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Ranking error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Ranking operation failed"
        )

@router.post("/similarity/cluster", response_model=ClusterResponse)
@limiter.limit("5/minute")
async def cluster_by_similarity(
    request: Request,
    req: ClusterRequest,
    api_key: str = Depends(verify_api_key)
):
    """
    Cluster sentences by semantic similarity using community detection.
    
    Groups similar sentences together based on a similarity threshold.
    """
    start_time = time.time()
    
    try:
        model = get_embedder()
        
        # Encode all sentences
        embeddings = model.encode(
            req.sentences,
            normalize_embeddings=True,
            batch_size=settings.BATCH_SIZE,
            show_progress_bar=False,
            convert_to_numpy=True
        )
        
        # Compute similarity matrix
        similarity_matrix = util.cos_sim(embeddings, embeddings)
        
        # Simple clustering: group sentences above threshold
        clusters_dict = {}
        assigned = set()
        
        for i in range(len(req.sentences)):
            if i in assigned:
                continue
            
            # Find all sentences similar to this one
            similar_indices = [
                j for j in range(len(req.sentences))
                if j not in assigned and similarity_matrix[i][j] >= req.threshold
            ]
            
            if similar_indices:
                cluster_members = [req.sentences[j] for j in similar_indices]
                avg_sim = float(np.mean([similarity_matrix[i][j] for j in similar_indices]))
                
                clusters_dict[i] = Cluster(
                    centroid_text=req.sentences[i],
                    members=cluster_members,
                    avg_similarity=round(avg_sim, 4)
                )
                
                assigned.update(similar_indices)
        
        processing_time = (time.time() - start_time) * 1000
        
        return ClusterResponse(
            clusters=list(clusters_dict.values()),
            num_clusters=len(clusters_dict),
            processing_time_ms=round(processing_time, 2)
        )
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Clustering error: {e}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail="Clustering operation failed"
        )
        