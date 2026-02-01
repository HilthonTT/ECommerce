from fastapi import Header, HTTPException, status
from config import settings
from typing import Optional

async def verify_api_key(x_api_key: Optional[str] = Header(None, alias=settings.API_KEY_HEADER)) -> str:
    """
    Verify API key if authentication is enabled.
    
    Returns the API key if valid, raises HTTPException if invalid.
    """
    
    if not settings.API_KEYS or settings.API_KEYS.strip() == "":
        return "no-auth-required"
    
    if not x_api_key:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="API key required",
            headers={"WWW-Authenticate": "ApiKey"},
        )
    
    valid_keys = [key.strip() for key in settings.API_KEYS.split(",")]
    
    if x_api_key not in valid_keys:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="Invalid API key"
        )
    
    return x_api_key
