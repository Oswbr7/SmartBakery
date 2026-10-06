from fastapi import APIRouter

from app.core.config import settings

router = APIRouter(tags=["health"])


@router.get("/health")
def health() -> dict:
    production_dir = settings.model_storage_path / "production"
    model_loaded = production_dir.exists() and any(production_dir.glob("*.joblib"))
    return {
        "status": "healthy",
        "service": settings.app_name,
        "modelLoaded": model_loaded,
    }
