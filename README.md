# Smart Bakery

Plataforma web para la gestión de una pastelería con predicción de demanda mediante Machine Learning.

Documentación completa en [`Docs/`](Docs/).

## Stack

| Componente | Tecnología | Carpeta | Puerto dev |
|---|---|---|---|
| Frontend | React + TypeScript + Vite, TanStack Query, Zustand, React Hook Form, shadcn/ui, Recharts | `frontend/smart-bakery-web` | 5173 |
| Backend | .NET 10, ASP.NET Core, EF Core, MediatR, FluentValidation, JWT (Clean Architecture) | `backend/SmartBakery` | 5001 (https) / 5000 |
| ML Service | Python, FastAPI, Pandas, NumPy, Scikit-learn, Joblib | `ml-service` | 8000 |
| Base de datos | PostgreSQL | — | 5432 |

## Prerequisitos

- Git
- .NET 10 SDK
- Node.js 20.19+ o 22.12+
- Python 3.11+
- PostgreSQL (local)

## Setup

```powershell
scripts\setup.cmd
```

El script verifica prerequisitos, instala paquetes NuGet, dependencias npm y el entorno virtual de Python, crea los `.env` desde `.env.example` e inicializa Git.

## Levantar servicios

```powershell
scripts\start-dev.cmd
```

O manualmente, una terminal por servicio:

```powershell
# ML Service
cd ml-service
.\.venv\Scripts\Activate.ps1
uvicorn app.main:app --reload --port 8000

# Backend
cd backend\SmartBakery
dotnet run --project src\SmartBakery.API --launch-profile https

# Frontend
cd frontend\smart-bakery-web
npm run dev
```

| Servicio | URL |
|---|---|
| Frontend | http://localhost:5173 |
| API (Swagger) | https://localhost:5001/swagger |
| API health | https://localhost:5001/health |
| ML (docs) | http://localhost:8000/docs |
| ML health | http://localhost:8000/health |

## Tests

```powershell
cd backend\SmartBakery ; dotnet test SmartBakery.slnx
cd frontend\smart-bakery-web ; npm run lint ; npm run test
cd ml-service ; .\.venv\Scripts\python -m pytest ; .\.venv\Scripts\ruff check .
```

## Estructura

```text
Smart Bakery/
├── Docs/                       # Especificación, arquitectura, API, DB, ML, MLOps, roadmap
├── backend/SmartBakery/        # .NET 10 - Clean Architecture
│   ├── src/  (API, Application, Domain, Infrastructure)
│   └── tests/ (UnitTests, IntegrationTests)
├── frontend/smart-bakery-web/  # React + Vite
├── ml-service/                 # FastAPI + scikit-learn
│   ├── app/  (api, core, ml, services)
│   ├── models/ (production, candidates)
│   ├── datasets/ (raw, processed)
│   ├── training/
│   └── tests/
├── scripts/                    # setup y arranque local
└── .github/workflows/          # CI (Fase 9)
```
