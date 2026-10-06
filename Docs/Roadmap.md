# 🗺️ Smart Bakery — Development Roadmap

## 1. Objetivo

Este documento define el orden recomendado para desarrollar **Smart Bakery**.

El proyecto tiene dos objetivos principales:

1. Construir una aplicación web funcional para administrar una pastelería.
2. Aprender y aplicar conceptos de:

   * Machine Learning.
   * MLOps.
   * CI/CD.
   * Clean Architecture.
   * CQRS.
   * APIs.
   * React.
   * .NET.
   * Python.
   * PostgreSQL.
   * Testing.
   * Docker.
   * Kubernetes.

Docker y Kubernetes son **opcionales** y no deben bloquear el desarrollo principal.

La aplicación debe funcionar completamente de forma nativa:

```text
React
.NET
Python
PostgreSQL
```

sin necesidad de Docker ni Kubernetes.

---

# 2. Filosofía del desarrollo

El proyecto debe construirse incrementalmente.

No intentar desarrollar todo simultáneamente.

El orden general será:

```text
Backend
   ↓
Database
   ↓
Frontend
   ↓
Business Features
   ↓
Historical Data
   ↓
Machine Learning
   ↓
ML API
   ↓
Integration
   ↓
Testing
   ↓
CI/CD
   ↓
MLOps
   ↓
Docker (Optional)
   ↓
Kubernetes (Optional)
```

---

# 3. Fases generales

```text
Phase 0  → Project Setup
Phase 1  → Database + Backend Foundation
Phase 2  → Authentication
Phase 3  → Products
Phase 4  → Sales
Phase 5  → Dashboard
Phase 6  → Synthetic Dataset
Phase 7  → First ML Model
Phase 8  → ML API
Phase 9  → .NET + ML Integration
Phase 10 → Predictions
Phase 11 → Model Versioning
Phase 12 → MLOps
Phase 13 → Testing
Phase 14 → CI/CD
Phase 15 → Documentation
Phase 16 → Docker Optional
Phase 17 → Kubernetes Optional
```

---

# 4. Phase 0 — Project Setup

## Objective

Crear la estructura inicial del repositorio.

---

## Tasks

Crear:

```text
smart-bakery/
│
├── frontend/
├── backend/
├── ml-service/
│
├── docs/
│
├── README.md
└── .gitignore
```

Documentación:

```text
docs/
├── PROJECT_SPECIFICATION.md
├── ARCHITECTURE.md
├── DATABASE.md
├── MACHINE_LEARNING.md
├── ROADMAP.md
├── API.md
├── CI_CD.md
├── MLOPS.md
└── DEVELOPMENT.md
```

---

## Backend

Crear solución:

```text
SmartBakery.sln
```

Proyectos:

```text
SmartBakery.API
SmartBakery.Application
SmartBakery.Domain
SmartBakery.Infrastructure
```

---

## Frontend

Crear aplicación:

```text
React
TypeScript
Vite
```

---

## ML

Crear:

```text
Python
FastAPI
```

---

## Git

Crear repositorio Git.

Primer commit:

```text
chore: initialize Smart Bakery project
```

---

## Definition of Done

```text
[ ] Repository created
[ ] Backend solution created
[ ] React project created
[ ] Python project created
[ ] PostgreSQL available locally
[ ] README created
[ ] Git initialized
```

---

# 5. Phase 1 — Database + Backend Foundation

## Objective

Construir la infraestructura principal del backend.

---

## Tasks

Implementar:

```text
Domain entities
EF Core
PostgreSQL
DbContext
Configurations
Migrations
Repositories
```

Entidades iniciales:

```text
Role
User
Category
Product
Promotion
Sale
SaleItem
Prediction
ModelVersion
```

---

## Architecture

Aplicar:

```text
Clean Architecture
```

con:

```text
Domain
Application
Infrastructure
API
```

---

## CQRS

Configurar:

```text
MediatR
```

Crear ejemplos:

```text
CreateProductCommand
GetProductsQuery
CreateSaleCommand
GetSalesQuery
```

---

## Validation

Configurar:

```text
FluentValidation
```

---

## Definition of Done

```text
[ ] PostgreSQL connected
[ ] EF Core configured
[ ] Initial migration created
[ ] Database created
[ ] Entities implemented
[ ] CQRS configured
[ ] FluentValidation configured
[ ] Global exception handling implemented
```

---

# 6. Phase 2 — Authentication

## Objective

Implementar autenticación y autorización.

---

## Tasks

Implementar:

```text
JWT
Password hashing
Login
User registration
Roles
Authorization
```

Roles:

```text
Admin
Employee
```

---

## Endpoints

```text
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

---

## Authorization

Ejemplo:

```text
Admin
    ↓
Can manage products
Can train models
Can manage users

Employee
    ↓
Can register sales
Can view products
Can view predictions
```

---

## Definition of Done

```text
[ ] User registration works
[ ] Login works
[ ] JWT generated
[ ] Protected endpoints work
[ ] Roles work
```

---

# 7. Phase 3 — Products

## Objective

Implementar administración de productos.

---

## Features

```text
Create product
Get products
Get product by ID
Update product
Deactivate product
```

---

## Categories

Implementar:

```text
Create category
Get categories
Update category
Deactivate category
```

---

## Frontend

Crear:

```text
Products page
Product form
Product details
Category selector
```

---

## Definition of Done

```text
[ ] Product CRUD works
[ ] Category CRUD works
[ ] Validation works
[ ] Products appear in React
[ ] Product can be deactivated
```

---

# 8. Phase 4 — Sales

## Objective

Implementar el registro de ventas.

---

## Backend

Crear:

```text
CreateSaleCommand
GetSalesQuery
GetSaleByIdQuery
```

---

## Sale flow

```text
Select products
      ↓
Specify quantities
      ↓
Calculate subtotal
      ↓
Apply promotion
      ↓
Calculate total
      ↓
Save Sale
      ↓
Save SaleItems
```

---

## Important rule

El precio del producto debe copiarse al `SaleItem`.

Ejemplo:

```text
Product.Price = 350

SaleItem.UnitPrice = 350
```

Si posteriormente cambia:

```text
Product.Price = 380
```

la venta histórica seguirá teniendo:

```text
SaleItem.UnitPrice = 350
```

---

## Frontend

Crear:

```text
Sales page
New sale form
Sale history
Sale details
```

---

## Definition of Done

```text
[ ] Sale can be created
[ ] Multiple products supported
[ ] Quantities supported
[ ] Subtotal calculated
[ ] Discounts supported
[ ] Total calculated
[ ] Sale history available
[ ] Historical prices preserved
```

---

# 9. Phase 5 — Dashboard

## Objective

Crear el dashboard principal.

---

## KPIs

Mostrar:

```text
Active Employees
Open Vacancies
Sales Today
Sales This Month
```

Para Smart Bakery, las métricas principales deberán adaptarse al negocio.

Ejemplo:

```text
Products
Sales Today
Revenue Today
Top Product
Predicted Demand
```

---

## Charts

Agregar:

```text
Sales over time
Sales by product
Sales by category
```

---

## Definition of Done

```text
[ ] Dashboard loads
[ ] KPIs work
[ ] Charts use real database data
[ ] Loading states implemented
[ ] Error states implemented
```

---

# 10. Phase 6 — Synthetic Dataset

## Objective

Generar suficientes datos históricos para comenzar a experimentar con Machine Learning.

---

## Dataset

Crear datos para:

```text
Products
Sales
SaleItems
Promotions
```

Periodo inicial sugerido:

```text
1–3 years
```

dependiendo de la cantidad de registros generados.

---

## Dataset characteristics

Debe incluir:

```text
Weekly seasonality
Monthly seasonality
Random variation
Promotions
Different product demand
Weekends
Trends
```

---

## Example

```text
Chocolate Cake

Monday:
12

Tuesday:
13

Wednesday:
14

Thursday:
15

Friday:
21

Saturday:
28

Sunday:
24
```

---

## Important

El dataset debe contener patrones suficientemente realistas para que Machine Learning tenga algo que aprender.

No debe generarse simplemente:

```text
random.randint(...)
```

sin estructura.

---

## Definition of Done

```text
[ ] Synthetic dataset generator exists
[ ] Dataset reproducible
[ ] Multiple products
[ ] Seasonal patterns
[ ] Promotions
[ ] Random variation
[ ] Historical dates
[ ] Dataset can populate PostgreSQL
```

---

# 11. Phase 7 — First Machine Learning Model

## Objective

Construir el primer pipeline de Machine Learning.

---

## Learning topics

Aprender:

```text
Supervised Learning
Regression
Features
Target
Training
Validation
Testing
Metrics
```

---

## Step 1 — Dataset

Extraer:

```text
Sales
SaleItems
Products
Promotions
```

---

## Step 2 — Feature Engineering

Crear:

```text
DayOfWeek
Month
IsWeekend
Price
Promotion
Lag1
Lag7
Lag14
RollingMean7
RollingMean30
```

---

## Step 3 — Baseline

Implementar:

```text
7-day average
```

---

## Step 4 — Linear Regression

Implementar:

```text
LinearRegression
```

---

## Step 5 — Random Forest

Implementar:

```text
RandomForestRegressor
```

---

## Step 6 — Evaluation

Medir:

```text
MAE
RMSE
R²
```

---

## Definition of Done

```text
[ ] Dataset generated
[ ] Features generated
[ ] No known data leakage
[ ] Baseline implemented
[ ] Linear Regression implemented
[ ] Random Forest implemented
[ ] Metrics calculated
[ ] Models compared
```

---

# 12. Phase 8 — ML API

## Objective

Convertir el pipeline de Machine Learning en un servicio consumible por HTTP.

---

## Technology

```text
Python
FastAPI
```

---

## Endpoints

```text
GET  /health
POST /predict
POST /train
GET  /model
GET  /metrics
```

---

## Prediction

Input:

```json
{
  "productId": "uuid",
  "date": "2026-10-15"
}
```

Output:

```json
{
  "prediction": 20.4,
  "modelVersion": "v3"
}
```

---

## Training

```text
POST /train
```

Debe:

```text
Load dataset
    ↓
Generate features
    ↓
Train
    ↓
Evaluate
    ↓
Save model
    ↓
Return metrics
```

---

## Definition of Done

```text
[ ] FastAPI runs
[ ] /health works
[ ] /predict works
[ ] /train works
[ ] Model can be loaded
[ ] Model can be serialized
[ ] Metrics returned
```

---

# 13. Phase 9 — .NET + ML Integration

## Objective

Conectar el backend principal con el ML service.

---

## Architecture

```text
React
   ↓
.NET
   ↓
Python FastAPI
   ↓
ML Model
```

React no deberá comunicarse directamente con Python.

---

## .NET

Crear:

```text
IMachineLearningClient
MachineLearningClient
```

Utilizando:

```text
HttpClient
```

---

## Flow

```text
React
  ↓
.NET API
  ↓
Application Layer
  ↓
IMachineLearningClient
  ↓
FastAPI
  ↓
Model
```

---

## Definition of Done

```text
[ ] .NET can call Python
[ ] Typed client implemented
[ ] Timeout configured
[ ] Errors handled
[ ] Python response mapped correctly
```

---

# 14. Phase 10 — Predictions

## Objective

Integrar las predicciones dentro de la aplicación.

---

## Features

Implementar:

```text
Generate prediction
View prediction
Save prediction
View prediction history
```

---

## Database

Utilizar:

```text
Predictions
ModelVersions
```

---

## Dashboard

Agregar:

```text
Predicted demand
```

Ejemplo:

```text
Chocolate Cake
Tomorrow

Predicted:
20 units
```

---

## Comparison

Posteriormente:

```text
Predicted
vs
Actual
```

---

## Definition of Done

```text
[ ] Prediction generated
[ ] Prediction persisted
[ ] Model version persisted
[ ] Prediction visible in React
[ ] Historical predictions available
```

---

# 15. Phase 11 — Model Versioning

## Objective

Implementar el ciclo de vida de los modelos.

---

## States

```text
Candidate
Production
Rejected
Retired
```

---

## Flow

```text
Training
    ↓
Candidate
    ↓
Evaluation
    ↓
Compare
    ↓
┌───────────────┐
│               │
▼               ▼
Production    Rejected
    │
    ▼
Previous Model
    ↓
Retired
```

---

## Metrics

Guardar:

```text
MAE
RMSE
R²
```

junto con:

```text
Algorithm
DatasetVersion
TrainingDate
ModelPath
```

---

## Definition of Done

```text
[ ] Model versions stored
[ ] Metrics stored
[ ] Candidate state exists
[ ] Production model exists
[ ] Rejected state exists
[ ] Retired state exists
[ ] Predictions reference model version
```

---

# 16. Phase 12 — MLOps

## Objective

Introducir prácticas de MLOps.

---

## Concepts

Aprender:

```text
Model lifecycle
Model registry
Dataset versioning
Experiment tracking
Model monitoring
Retraining
Model promotion
```

---

## Initial implementation

Sin herramientas externas:

```text
PostgreSQL
+
JSON/metadata
+
joblib
```

---

## Advanced implementation

Opcionalmente:

```text
MLflow
```

---

## Experiment tracking

Registrar:

```text
Experiment
Dataset
Features
Algorithm
Hyperparameters
Metrics
Date
Result
```

---

## Model promotion

Regla inicial:

```text
New MAE < Production MAE
```

Además:

```text
Dataset valid
Model valid
Prediction works
No critical errors
```

---

## Definition of Done

```text
[ ] Experiments tracked
[ ] Dataset versions identifiable
[ ] Model versions tracked
[ ] Candidate models evaluated
[ ] Production model identifiable
[ ] Retraining works
[ ] Model promotion process exists
```

---

# 17. Phase 13 — Testing

## Objective

Aumentar la confiabilidad de todo el sistema.

---

# Backend tests

Implementar:

```text
Unit Tests
Integration Tests
```

Testear:

```text
Commands
Queries
Validators
Business rules
Repositories
Controllers
```

---

# Frontend tests

Testear:

```text
Components
Forms
Hooks
Critical user flows
```

---

# ML tests

Testear:

```text
Feature engineering
Dataset validation
Training
Model loading
Prediction
```

---

# Integration tests

Validar:

```text
React
 ↓
.NET
 ↓
PostgreSQL
```

y:

```text
.NET
 ↓
Python
```

---

## Definition of Done

```text
[ ] Backend unit tests
[ ] Backend integration tests
[ ] Frontend critical tests
[ ] ML tests
[ ] Integration tests
```

---

# 18. Phase 14 — CI/CD

## Objective

Automatizar validaciones y despliegues mediante GitHub Actions.

---

## Pull Request pipeline

Cada Pull Request deberá ejecutar:

```text
Checkout
   ↓
Restore
   ↓
Build
   ↓
Test
   ↓
Lint
```

---

# Backend

```text
dotnet restore
dotnet build
dotnet test
```

---

# Frontend

```text
npm ci
npm run lint
npm run test
npm run build
```

---

# ML

```text
pip install
pytest
```

---

# CI architecture

```text
GitHub
   │
   ▼
Pull Request
   │
   ▼
GitHub Actions
   │
   ├── Backend
   ├── Frontend
   └── ML
```

---

## Definition of Done

```text
[ ] CI workflow exists
[ ] Backend builds
[ ] Backend tests execute
[ ] Frontend builds
[ ] Frontend tests execute
[ ] ML tests execute
[ ] PR checks work
```

---

# 19. Phase 15 — Documentation

## Objective

Documentar correctamente el proyecto.

---

## Required documents

```text
README.md
PROJECT_SPECIFICATION.md
ARCHITECTURE.md
DATABASE.md
MACHINE_LEARNING.md
ROADMAP.md
API.md
CI_CD.md
MLOPS.md
DEVELOPMENT.md
```

---

## README

Debe explicar:

```text
What is Smart Bakery?
Features
Architecture
Technologies
How to run
Screenshots
ML overview
CI/CD
Future improvements
```

---

# 20. Phase 16 — Docker Optional

## Objective

Agregar containerización una vez que la aplicación funcione correctamente de forma nativa.

Docker debe considerarse una **capa de infraestructura**, no un requisito para desarrollar el proyecto.

---

## Containers

Posibles servicios:

```text
frontend
backend
ml-service
postgres
```

---

## Docker Compose

Posteriormente:

```text
docker-compose.yml
```

Conceptualmente:

```text
┌──────────────┐
│   Frontend   │
└──────┬───────┘
       │
       ▼
┌──────────────┐
│   Backend    │
└──────┬───────┘
       │
   ┌───┴────┐
   ▼        ▼
Postgres   ML
```

---

## Important

Docker no debe ser necesario para:

```text
Development
Testing
ML experimentation
```

durante las primeras fases.

---

## Definition of Done

```text
[ ] Dockerfiles created
[ ] Services can build
[ ] Docker Compose works
[ ] Environment variables configured
[ ] Services communicate correctly
```

---

# 21. Phase 17 — Kubernetes Optional

## Objective

Aprender conceptos básicos de Kubernetes una vez que Docker esté funcionando.

---

## Possible resources

```text
Deployment
Service
ConfigMap
Secret
Ingress
PersistentVolume
PersistentVolumeClaim
```

---

## Architecture

```text
Kubernetes Cluster
│
├── Frontend
│
├── Backend
│
├── ML Service
│
└── PostgreSQL
```

---

## Important

Kubernetes no debe introducirse antes de comprender:

```text
Application
Docker
Networking
Configuration
Health checks
```

---

## Definition of Done

```text
[ ] Kubernetes manifests created
[ ] Frontend deployment works
[ ] Backend deployment works
[ ] ML deployment works
[ ] Services communicate
[ ] Configuration separated
[ ] Health checks configured
```

---

# 22. Recommended implementation order

La secuencia recomendada es:

```text
1. Repository
        ↓
2. .NET Solution
        ↓
3. PostgreSQL
        ↓
4. EF Core
        ↓
5. Domain
        ↓
6. Application
        ↓
7. Authentication
        ↓
8. Products
        ↓
9. Sales
        ↓
10. Dashboard
        ↓
11. Synthetic Data
        ↓
12. ML Dataset
        ↓
13. Baseline
        ↓
14. Linear Regression
        ↓
15. Random Forest
        ↓
16. ML Evaluation
        ↓
17. FastAPI
        ↓
18. .NET ↔ Python
        ↓
19. Predictions
        ↓
20. Model Versioning
        ↓
21. MLOps
        ↓
22. Testing
        ↓
23. CI/CD
        ↓
24. Documentation
        ↓
25. Docker [OPTIONAL]
        ↓
26. Kubernetes [OPTIONAL]
```

---

# 23. MVP Definition

El MVP no necesita todas las fases.

El MVP debe llegar hasta:

```text
Sales
   ↓
Historical Data
   ↓
ML
   ↓
Prediction
   ↓
Dashboard
```

Específicamente:

```text
[ ] Authentication
[ ] Products
[ ] Categories
[ ] Sales
[ ] Dashboard
[ ] Synthetic dataset
[ ] Feature engineering
[ ] Baseline
[ ] Linear Regression
[ ] Random Forest
[ ] Evaluation
[ ] FastAPI
[ ] .NET integration
[ ] Predictions
```

---

# 24. Version 1.0

Después del MVP:

```text
[ ] Model versioning
[ ] Retraining
[ ] Experiment tracking
[ ] CI/CD
[ ] Testing improvements
[ ] Monitoring
[ ] Better dashboard
```

---

# 25. Version 2.0

Características avanzadas:

```text
[ ] MLflow
[ ] Advanced models
[ ] Prediction intervals
[ ] SHAP
[ ] Dataset versioning
[ ] Automated retraining
[ ] Automated model promotion
```

---

# 26. Infrastructure version

Opcionalmente:

```text
[ ] Docker
[ ] Docker Compose
[ ] Kubernetes
[ ] Cloud deployment
[ ] Object storage for models
```

Estas características no forman parte del núcleo funcional de Smart Bakery.

---

# 27. Learning progression

El proyecto está diseñado para aprender progresivamente.

## Level 1 — Software Engineering

```text
C#
.NET
React
PostgreSQL
EF Core
REST
Clean Architecture
CQRS
```

---

## Level 2 — Machine Learning

```text
Python
Pandas
NumPy
scikit-learn
Regression
Features
Training
Evaluation
```

---

## Level 3 — Applied ML

```text
Feature Engineering
Time Series
Forecasting
Data Leakage
Overfitting
Hyperparameters
Model Comparison
```

---

## Level 4 — MLOps

```text
Model Versioning
Experiment Tracking
Model Registry
Retraining
Monitoring
Deployment
```

---

## Level 5 — DevOps

```text
GitHub Actions
CI/CD
Docker
Containerization
Kubernetes
```

---

# 28. Scope control

Para evitar que el proyecto crezca indefinidamente, las siguientes funcionalidades deben considerarse fuera del MVP:

```text
Inventory management
Supplier management
Payments
Customer accounts
Mobile application
Weather API
External event APIs
Advanced forecasting models
Real-time online learning
Microservices for every business module
Kubernetes
Cloud infrastructure
```

Estas funcionalidades podrán agregarse después si existe una razón técnica o de aprendizaje.

---

# 29. What should NOT happen

No se debe:

```text
✗ Implementar Kubernetes antes del MVP.
✗ Implementar MLflow antes de entender el pipeline.
✗ Utilizar XGBoost sin haber probado un baseline.
✗ Crear microservicios innecesarios.
✗ Entrenar modelos sin separar temporalmente los datos.
✗ Utilizar datos futuros para crear features históricas.
✗ Permitir que React acceda directamente a PostgreSQL.
✗ Permitir que React dependa directamente del ML service.
✗ Mezclar lógica de negocio con código de Machine Learning.
✗ Hacer que Docker sea obligatorio para desarrollar.
```

---

# 30. Milestones

## Milestone 1 — Backend Foundation

```text
.NET + PostgreSQL + Clean Architecture
```

Resultado:

```text
Backend funcional
```

---

## Milestone 2 — Bakery Management

```text
Products + Sales + Dashboard
```

Resultado:

```text
Aplicación de administración funcional
```

---

## Milestone 3 — First ML

```text
Dataset + Baseline + Models
```

Resultado:

```text
Primer modelo capaz de generar predicciones
```

---

## Milestone 4 — ML Integration

```text
FastAPI + .NET
```

Resultado:

```text
Aplicación web consumiendo Machine Learning
```

---

## Milestone 5 — MLOps

```text
Versioning + Retraining + Experiments
```

Resultado:

```text
Ciclo de vida del modelo
```

---

## Milestone 6 — CI/CD

```text
GitHub Actions
```

Resultado:

```text
Automated validation
```

---

## Milestone 7 — Optional Infrastructure

```text
Docker
Kubernetes
```

Resultado:

```text
Containerized / orchestrated application
```

---

# 31. Final architecture goal

La arquitectura final esperada:

```text
                         ┌────────────────────┐
                         │       React        │
                         │    TypeScript      │
                         └─────────┬──────────┘
                                   │
                                   ▼
                         ┌────────────────────┐
                         │      .NET 10       │
                         │   ASP.NET Core     │
                         │                    │
                         │ Clean Architecture │
                         │ CQRS / MediatR     │
                         └───────┬─────┬──────┘
                                 │     │
                        ┌────────┘     └─────────┐
                        ▼                        ▼
               ┌────────────────┐       ┌────────────────┐
               │   PostgreSQL   │       │  Python ML     │
               │                │       │    FastAPI     │
               │ Business Data  │       │                │
               │ Predictions    │       │ Training       │
               │ Models         │       │ Prediction     │
               └────────────────┘       └───────┬────────┘
                                                │
                                                ▼
                                       ┌────────────────┐
                                       │ Model Artifact │
                                       │    .joblib     │
                                       └────────────────┘
```

CI/CD:

```text
                    GitHub
                       │
                       ▼
                GitHub Actions
                       │
             ┌─────────┼─────────┐
             ▼         ▼         ▼
          Backend   Frontend     ML
             │         │         │
             └─────────┼─────────┘
                       ▼
                    Tests
                       │
                       ▼
                    Build
```

---

# 32. Final success criteria

Smart Bakery podrá considerarse un proyecto completo cuando:

```text
[ ] La aplicación permite administrar productos.
[ ] La aplicación permite registrar ventas.
[ ] Existe historial suficiente de ventas.
[ ] El sistema puede generar un dataset.
[ ] Existe un pipeline reproducible de Machine Learning.
[ ] Existe un baseline.
[ ] Se han probado múltiples modelos.
[ ] Los modelos se evalúan correctamente.
[ ] No existe data leakage conocido.
[ ] El mejor candidato puede convertirse en Production.
[ ] Los modelos están versionados.
[ ] El sistema puede generar predicciones.
[ ] Las predicciones se almacenan.
[ ] El dashboard muestra resultados.
[ ] El modelo puede reentrenarse.
[ ] Existen pruebas automatizadas.
[ ] Existe CI/CD.
[ ] La documentación está actualizada.
[ ] Docker puede agregarse sin modificar la lógica de negocio.
[ ] Kubernetes puede agregarse sin modificar la lógica de negocio.
```

---

# 33. Principle of completion

> **Primero hacer que funcione. Después hacerlo correcto. Después hacerlo automatizado. Finalmente hacerlo escalable.**

La prioridad será:

```text
Functional
    ↓
Correct
    ↓
Tested
    ↓
Automated
    ↓
Observable
    ↓
Scalable
```

Smart Bakery debe ser primero una aplicación funcional y un pipeline de Machine Learning comprensible. Las tecnologías de infraestructura avanzada deben incorporarse únicamente después de que el núcleo del sistema esté funcionando correctamente.
