from pathlib import Path

from pydantic_settings import BaseSettings, SettingsConfigDict

BASE_DIR = Path(__file__).resolve().parents[2]


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=BASE_DIR / ".env", extra="ignore")

    app_name: str = "Smart Bakery ML Service"
    environment: str = "Development"
    model_storage_path: Path = BASE_DIR / "models"
    dataset_path: Path = BASE_DIR / "datasets"
    ml_service_api_key: str = ""


settings = Settings()
