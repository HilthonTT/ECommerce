from pydantic_settings import BaseSettings
from functools import lru_cache

class Settings(BaseSettings):
    """Application settings from environment variables"""
    
    # Server settings
    HOST: str = "0.0.0.0"
    PORT: int = 8000
    DEBUG: bool = False
    LOG_LEVEL: str = "INFO"
    
    # Security
    ALLOWED_HOSTS: str = ""  # Comma-separated list, empty means all
    CORS_ORIGINS: str = "*"  # Comma-separated origins or "*"
    API_KEY_HEADER: str = "X-API-Key"
    API_KEYS: str = ""  # Comma-separated API keys, empty means no auth required
    
    # Features
    ENABLE_DOCS: bool = True
    
    # Rate limiting (per minute)
    RATE_LIMIT_CLASSIFY: str = "10/minute"
    RATE_LIMIT_EMBED: str = "20/minute"
    RATE_LIMIT_SIMILARITY: str = "15/minute"
    RATE_LIMIT_GENERAL: str = "100/minute"
    
    # Model settings
    CLASSIFIER_MODEL: str = "cross-encoder/nli-MiniLM2-L6-H768"
    EMBEDDING_MODEL: str = "sentence-transformers/all-MiniLM-L6-v2"
    MODEL_CACHE_DIR: str = "./model_cache"
    
    # Input validation
    MAX_TEXT_LENGTH: int = 5000
    MAX_SENTENCES_EMBED: int = 100
    MIN_CANDIDATE_LABELS: int = 2
    MAX_CANDIDATE_LABELS: int = 20
    
    class Config:
        env_file = ".env"
        case_sensitive = True

@lru_cache()
def get_settings() -> Settings:
    return Settings()

settings = get_settings()
