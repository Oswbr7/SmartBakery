# 🏗️ Smart Bakery — Architecture

## 1. Objetivo

Este documento define la arquitectura técnica de **Smart Bakery**, una plataforma web para la gestión de una pastelería y la predicción de demanda mediante Machine Learning.

La arquitectura está diseñada para:

* Separar responsabilidades.
* Mantener el código desacoplado.
* Permitir evolución independiente de cada componente.
* Facilitar testing.
* Permitir CI/CD.
* Separar la lógica de negocio de Machine Learning.
* Permitir incorporar Docker y Kubernetes posteriormente sin modificar la arquitectura de aplicación.

---

# 2. Arquitectura general

Smart Bakery estará compuesto inicialmente por tres aplicaciones principales:

```text
┌─────────────────────────────────────────────────────────────┐
│                         SMART BAKERY                         │
│                                                             │
│  ┌─────────────────┐                                        │
│  │     React       │                                        │
│  │    Frontend     │                                        │
│  └────────┬────────┘                                        │
│           │ HTTP/HTTPS                                      │
│           ▼                                                 │
│  ┌───────────────────────────────────────────────────────┐  │
│  │                    .NET 10 API                        │  │
│  │                                                       │  │
│  │  Authentication                                       │  │
│  │  Products                                             │  │
│  │  Sales                                                │  │
│  │  Dashboard                                            │  │
│  │  Predictions                                          │  │
│  │  Model Management                                     │  │
│  └───────────────┬───────────────────────┬───────────────┘  │
│                  │                       │                  │
│                  │                       │ HTTP             │
│                  │                       ▼                  │
│                  │             ┌──────────────────────┐   │
│                  │             │    Python ML API     │   │
│                  │             │       FastAPI        │   │
│                  │             │                      │   │
│                  │             │ Prediction           │   │
│                  │             │ Training             │   │
│                  │             │ Evaluation           │   │
│                  │             └──────────┬───────────┘   │
│                  │                        │               │
│                  ▼                        ▼               │
│          ┌──────────────────┐    ┌────────────────────┐  │
│          │   PostgreSQL     │    │    ML Models       │  │
│          │                  │    │                    │  │
│          │ Products         │    │ model_v1.joblib    │  │
│          │ Sales            │    │ model_v2.joblib    │  │
│          │ Predictions      │    │ model_v3.joblib    │  │
│          │ Model metadata   │    │                    │  │
│          └──────────────────┘    └────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

# 3. Architectural style

El proyecto utilizará una combinación de:

* **Modular Monolith** para el backend principal.
* **Clean Architecture** dentro del backend.
* **Microservice independiente** para Machine Learning.
* **Client-Server Architecture** entre frontend y backend.
* **REST APIs** para comunicación entre componentes.

No se pretende convertir todo el sistema en microservicios.

La única separación inicial obligatoria será la del servicio de Machine Learning debido a las diferencias entre el ecosistema .NET y Python.

---

# 4. Principio de separación

Cada componente tendrá una responsabilidad principal.

```text
React
   │
   └── Presentación y experiencia de usuario

.NET API
   │
   └── Reglas de negocio y API principal

PostgreSQL
   │
   └── Persistencia

Python ML Service
   │
   └── Machine Learning

GitHub Actions
   │
   └── Automatización CI/CD
```

Ningún componente debe asumir responsabilidades pertenecientes a otro.

---

# 5. Frontend

## 5.1 Tecnología

* React
* TypeScript
* Vite
* TanStack Query
* Zustand
* React Hook Form
* shadcn/ui
* Recharts

---

# 5.2 Responsabilidad

El frontend es responsable exclusivamente de:

* Renderizar interfaces.
* Gestionar interacción del usuario.
* Validaciones básicas de formularios.
* Consumir la API.
* Mostrar errores.
* Mostrar estados de carga.
* Visualizar datos y gráficas.
* Gestionar el estado de UI.

El frontend **no debe contener lógica de negocio crítica**.

---

# 5.3 Comunicación

El frontend se comunicará únicamente con la API .NET.

```text
React
  │
  │ HTTP/HTTPS
  ▼
.NET API
```

No se permitirá:

```text
React ──────────► PostgreSQL
```

ni:

```text
React ──────────► Python ML API
```

El frontend no debe conocer detalles internos de infraestructura.

---

# 5.4 Estructura

```text
frontend/
└── smart-bakery-web/
    │
    ├── src/
    │   │
    │   ├── components/
    │   ├── features/
    │   │   ├── auth/
    │   │   ├── products/
    │   │   ├── sales/
    │   │   ├── dashboard/
    │   │   └── predictions/
    │   │
    │   ├── hooks/
    │   ├── layouts/
    │   ├── lib/
    │   ├── services/
    │   ├── stores/
    │   ├── types/
    │   ├── routes/
    │   └── App.tsx
    │
    ├── public/
    ├── tests/
    └── package.json
```

---

# 6. Backend

## 6.1 Tecnología

* C#
* .NET 10
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* MediatR
* FluentValidation
* JWT

---

# 6.2 Responsabilidad

El backend representa el núcleo de negocio de Smart Bakery.

Será responsable de:

* Autenticación.
* Autorización.
* Gestión de productos.
* Gestión de ventas.
* Gestión de usuarios.
* Dashboard.
* Persistencia.
* Reglas de negocio.
* Comunicación con Python.
* Gestión de predicciones.
* Gestión de metadata de modelos.
* Validaciones.
* Manejo global de errores.

---

# 7. Clean Architecture

El backend seguirá una estructura basada en Clean Architecture.

```text
┌───────────────────────────────────────┐
│             Presentation              │
│             ASP.NET API               │
├───────────────────────────────────────┤
│              Application              │
│         Use Cases / CQRS              │
├───────────────────────────────────────┤
│                Domain                 │
│        Entities / Business Rules      │
├───────────────────────────────────────┤
│            Infrastructure             │
│ EF Core / PostgreSQL / External APIs  │
└───────────────────────────────────────┘
```

La dependencia debe apuntar hacia el interior.

```text
Presentation
     ↓
Application
     ↓
Domain

Infrastructure
     ↓
Application / Domain
```

El Domain no debe depender de Infrastructure.

---

# 8. Backend structure

```text
backend/
└── SmartBakery/
    │
    ├── src/
    │   │
    │   ├── SmartBakery.API/
    │   │
    │   ├── SmartBakery.Application/
    │   │
    │   ├── SmartBakery.Domain/
    │   │
    │   └── SmartBakery.Infrastructure/
    │
    ├── tests/
    │   │
    │   ├── SmartBakery.UnitTests/
    │   └── SmartBakery.IntegrationTests/
    │
    └── SmartBakery.sln
```

---

# 9. Domain Layer

El Domain contiene las reglas y entidades fundamentales del negocio.

Ejemplos:

```text
Product
Sale
SaleItem
User
Role
Prediction
ModelVersion
Promotion
```

El Domain no debe conocer:

* ASP.NET.
* Entity Framework.
* PostgreSQL.
* HTTP.
* FastAPI.
* React.
* JWT implementation details.

---

# 10. Application Layer

Application contiene los casos de uso.

Se utilizará CQRS.

Ejemplo:

```text
Products
├── Commands
│   ├── CreateProduct
│   ├── UpdateProduct
│   └── DeactivateProduct
│
└── Queries
    ├── GetProduct
    └── GetProducts
```

Ventas:

```text
Sales
├── Commands
│   └── RegisterSale
│
└── Queries
    ├── GetSale
    └── GetSales
```

Predicciones:

```text
Predictions
├── Commands
│   └── RequestPrediction
│
└── Queries
    ├── GetPrediction
    └── GetPredictions
```

Modelos:

```text
Models
├── Commands
│   └── TrainModel
│
└── Queries
    ├── GetCurrentModel
    └── GetModelMetrics
```

---

# 11. Infrastructure Layer

Infrastructure implementará las dependencias externas.

Responsabilidades:

* Entity Framework Core.
* PostgreSQL.
* Repositories.
* JWT.
* HTTP clients.
* File system.
* Comunicación con Python.
* Logging.
* Servicios externos.

Ejemplo:

```text
Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── Migrations/
│
├── Repositories/
│
├── Authentication/
│
├── MachineLearning/
│   └── MachineLearningClient.cs
│
└── Services/
```

---

# 12. API Layer

ASP.NET Core será responsable de exponer los endpoints HTTP.

Ejemplo:

```text
/api/auth
/api/products
/api/sales
/api/dashboard
/api/predictions
/api/models
```

Los Controllers deben ser delgados.

Ejemplo conceptual:

```text
HTTP Request
     ↓
Controller
     ↓
Command / Query
     ↓
Handler
     ↓
Domain
     ↓
Infrastructure
     ↓
Response
```

El Controller no debe contener lógica de negocio compleja.

---

# 13. PostgreSQL

PostgreSQL será la base de datos principal.

Será utilizada para almacenar:

* Usuarios.
* Productos.
* Ventas.
* Detalles de ventas.
* Promociones.
* Predicciones.
* Versiones de modelos.
* Métricas.
* Metadata.

---

# 14. Responsabilidad de PostgreSQL

PostgreSQL almacenará información estructurada del negocio.

No debe almacenar directamente el modelo de Machine Learning como parte del MVP.

Por ejemplo:

```text
PostgreSQL
      │
      ├── ModelVersion
      │     ├── Version
      │     ├── MAE
      │     ├── RMSE
      │     ├── R2
      │     └── Path
      │
      └── Prediction
            ├── Product
            ├── Date
            ├── Quantity
            └── ModelVersion
```

El archivo físico del modelo permanecerá fuera de la base de datos.

---

# 15. Machine Learning Service

El servicio de Machine Learning será una aplicación independiente desarrollada en Python.

Tecnologías:

* Python
* FastAPI
* Pandas
* NumPy
* Scikit-learn
* Joblib
* pytest

Opcional posteriormente:

* MLflow
* XGBoost
* LightGBM

---

# 16. Responsabilidades del ML Service

El servicio Python será responsable exclusivamente de:

* Preparación de datos para ML.
* Feature engineering.
* Entrenamiento.
* Evaluación.
* Predicción.
* Persistencia de modelos.
* Carga de modelos.
* Comparación de métricas.

No debe contener reglas de negocio propias de la pastelería.

Por ejemplo:

```text
Python puede saber:
"el modelo predice 17 unidades"

Python NO debería decidir:
"la pastelería debe fabricar 17 pasteles"
```

La interpretación de la predicción pertenece al backend/aplicación.

---

# 17. ML Service structure

```text
ml-service/
│
├── app/
│   ├── main.py
│   │
│   ├── api/
│   │   ├── routes/
│   │   │   ├── health.py
│   │   │   ├── prediction.py
│   │   │   ├── training.py
│   │   │   └── models.py
│   │   │
│   │   └── schemas/
│   │
│   ├── core/
│   │   └── config.py
│   │
│   ├── ml/
│   │   ├── preprocessing/
│   │   ├── features/
│   │   ├── training/
│   │   ├── evaluation/
│   │   └── prediction/
│   │
│   └── services/
│
├── models/
│   ├── model_v1.joblib
│   └── model_v2.joblib
│
├── datasets/
│
├── tests/
│
├── requirements.txt
└── README.md
```

---

# 18. Comunicación .NET ↔ Python

El backend .NET será el único componente autorizado para comunicarse con el ML Service.

```text
.NET API
   │
   │ HTTP
   ▼
FastAPI
   │
   ▼
ML Model
```

Se utilizará un `HttpClient` tipado en .NET.

Ejemplo conceptual:

```text
IMachineLearningClient
        │
        ▼
MachineLearningClient
        │
        ▼
FastAPI
```

Esto permitirá reemplazar la implementación posteriormente sin afectar Application.

---

# 19. Flujo de predicción

Cuando el usuario solicite una predicción:

```text
┌──────────────┐
│    React     │
└──────┬───────┘
       │
       │ POST /api/predictions
       ▼
┌──────────────┐
│  .NET API    │
└──────┬───────┘
       │
       │ Validate request
       ▼
┌──────────────┐
│ Application  │
└──────┬───────┘
       │
       │ Request prediction
       ▼
┌──────────────┐
│ Python API   │
└──────┬───────┘
       │
       ▼
┌──────────────┐
│ ML Model     │
└──────┬───────┘
       │
       │ Prediction
       ▼
┌──────────────┐
│  .NET API    │
└──────┬───────┘
       │
       ▼
 PostgreSQL
       │
       ▼
     React
```

---

# 20. Flujo de entrenamiento

El entrenamiento será diferente a una predicción.

```text
Admin
  │
  ▼
React
  │
  ▼
.NET API
  │
  ▼
Training Command
  │
  ▼
Python ML Service
  │
  ├── Load Dataset
  │
  ├── Clean Data
  │
  ├── Feature Engineering
  │
  ├── Train
  │
  ├── Evaluate
  │
  └── Save Model
  │
  ▼
Metrics
  │
  ▼
.NET
  │
  ▼
Compare with production
  │
  ├── Worse → Keep current model
  │
  └── Better → Register new model
```

---

# 21. Model Storage

Durante el desarrollo local se utilizará almacenamiento basado en archivos.

```text
ml-service/
└── models/
    ├── model_v1.joblib
    ├── model_v2.joblib
    └── model_v3.joblib
```

La base de datos almacenará metadata:

```text
ModelVersion
----------------
Id
Version
Algorithm
MAE
RMSE
R2
DatasetVersion
FilePath
TrainingDate
IsProduction
```

Posteriormente se podrá reemplazar el almacenamiento por:

* S3.
* Azure Blob Storage.
* MinIO.
* Otro object storage.

La aplicación deberá utilizar una abstracción para evitar acoplamiento con el proveedor.

---

# 22. Prediction persistence

Cada predicción generada debe poder registrarse.

```text
Prediction
----------------
Id
ProductId
PredictionDate
PredictedQuantity
ModelVersionId
CreatedAt
```

Cuando existan ventas reales posteriormente, será posible comparar:

```text
Predicción
    vs
Venta real
```

Esto permitirá analizar el rendimiento real del modelo después de su despliegue.

---

# 23. Model lifecycle

Los modelos tendrán un ciclo de vida.

```text
Training
   │
   ▼
Candidate
   │
   ▼
Evaluation
   │
   ├───────────────┐
   ▼               ▼
Rejected        Approved
                   │
                   ▼
               Production
                   │
                   ▼
                Retired
```

Estados:

```text
Candidate
Production
Rejected
Retired
```

Solo puede existir un modelo principal en estado `Production` para una determinada estrategia de predicción.

---

# 24. Seguridad entre servicios

En desarrollo local:

```text
.NET → Python
```

podrá utilizar una configuración sencilla.

En ambientes posteriores se recomienda:

* API Key interna.
* HTTPS.
* Secrets.
* Restricción de acceso de red.

El endpoint de entrenamiento debe estar protegido.

---

# 25. Authentication

El frontend autenticará usuarios contra .NET.

```text
React
  │
  │ Login
  ▼
.NET API
  │
  ▼
JWT
  │
  ▼
React
```

Posteriormente el token será enviado en:

```http
Authorization: Bearer <token>
```

El ML Service no manejará usuarios ni autenticación de usuarios.

---

# 26. Roles

Roles iniciales:

```text
Admin
Employee
```

### Admin

Puede:

* Administrar productos.
* Consultar ventas.
* Consultar dashboard.
* Consultar modelos.
* Entrenar modelos.
* Consultar métricas.

### Employee

Puede:

* Consultar productos.
* Registrar ventas.
* Consultar información permitida del dashboard.
* Consultar predicciones.

---

# 27. Error handling

El backend utilizará un middleware global para excepciones.

Flujo:

```text
Exception
   ↓
Global Exception Middleware
   ↓
Map exception
   ↓
Standard API Response
```

Formato:

```json
{
  "status": 400,
  "message": "Unable to register sale.",
  "errors": []
}
```

El ML Service deberá utilizar respuestas HTTP consistentes.

---

# 28. Health Checks

La aplicación deberá tener health checks.

## .NET

```http
GET /health
```

Debe verificar como mínimo:

* Aplicación funcionando.
* Conectividad con PostgreSQL.

## Python

```http
GET /health
```

Debe verificar:

* Aplicación funcionando.
* Estado del modelo cargado.

---

# 29. Configuración

No se deben hardcodear:

* Connection strings.
* JWT secrets.
* ML API URLs.
* API keys.
* Paths sensibles.

Utilizar variables de entorno.

Ejemplo:

```text
DATABASE_CONNECTION_STRING
JWT_SECRET
ML_SERVICE_URL
ML_SERVICE_API_KEY
MODEL_STORAGE_PATH
```

En desarrollo se podrá utilizar:

```text
.env
```

sin subir secretos al repositorio.

---

# 30. Entornos

Se contemplan inicialmente:

```text
Development
Test
Production
```

### Development

Uso local.

```text
React
.NET
Python
PostgreSQL
```

### Test

Utilizado por CI/CD.

Debe ejecutar:

* Unit tests.
* Integration tests.
* ML tests.

### Production

Será definido posteriormente.

No se debe asumir inicialmente un proveedor cloud.

---

# 31. Desarrollo local

El proyecto debe poder ejecutarse sin Docker.

Procesos:

```text
Terminal 1
---------
React
npm run dev


Terminal 2
---------
.NET
dotnet run


Terminal 3
---------
Python
uvicorn app.main:app --reload


Terminal 4
---------
PostgreSQL
Local PostgreSQL instance
```

Cada servicio tendrá configuración independiente.

---

# 32. Docker como capa opcional

Docker no forma parte de la arquitectura obligatoria.

Cuando se implemente:

```text
React
  ↓
Docker Container

.NET
  ↓
Docker Container

Python
  ↓
Docker Container

PostgreSQL
  ↓
Docker Container
```

La lógica de aplicación no deberá depender de Docker.

---

# 33. Kubernetes como capa opcional

Kubernetes será considerado una capa posterior de infraestructura.

La arquitectura de aplicación deberá permanecer igual:

```text
React
  ↓
.NET
  ↓
Python
  ↓
PostgreSQL
```

Kubernetes únicamente cambiará la forma en que estos componentes son ejecutados y administrados.

---

# 34. CI/CD

GitHub Actions será responsable de automatizar:

```text
Code
 ↓
Build
 ↓
Test
 ↓
Validate
 ↓
Package
 ↓
Deploy
```

No será necesario utilizar Docker para los pipelines iniciales.

---

# 35. CI Pipeline

El pipeline debe validar tres componentes.

```text
             Git Push
                │
       ┌────────┼────────┐
       ▼        ▼        ▼
    Frontend  Backend    ML
       │        │        │
       ▼        ▼        ▼
    Tests     Tests     Tests
       │        │        │
       └────────┼────────┘
                ▼
              Build
                │
                ▼
             Success
```

---

# 36. Dependencias entre componentes

```text
React
  │
  └── depends on → .NET API

.NET API
  │
  ├── depends on → PostgreSQL
  │
  └── depends on → Python ML API

Python ML API
  │
  └── depends on → Model Storage

GitHub Actions
  │
  └── validates all components
```

No debe existir dependencia circular.

---

# 37. Reglas arquitectónicas

## Regla 1

React no accede directamente a PostgreSQL.

## Regla 2

React no accede directamente al ML Service.

## Regla 3

Domain no depende de Infrastructure.

## Regla 4

Controllers no contienen reglas de negocio complejas.

## Regla 5

Python no contiene reglas de negocio de la pastelería.

## Regla 6

El modelo ML no debe modificar directamente PostgreSQL.

## Regla 7

El ML Service no maneja autenticación de usuarios.

## Regla 8

Docker no debe ser requisito para ejecutar la aplicación.

## Regla 9

Kubernetes no debe ser requisito para completar el proyecto.

## Regla 10

Las dependencias externas deben abstraerse mediante interfaces cuando sea razonable.

---

# 38. Evolución futura

La arquitectura debe permitir evolucionar hacia:

```text
                 Cloud
                   │
              ┌────┴────┐
              │         │
           Backend      ML
              │         │
              └────┬────┘
                   │
              PostgreSQL
```

Y posteriormente:

```text
                Kubernetes
                     │
       ┌─────────────┼─────────────┐
       ▼             ▼             ▼
    Frontend       Backend        ML
                                   │
                                   ▼
                              Model Storage
                     │
                     ▼
                PostgreSQL
```

Sin modificar las reglas principales del dominio.

---

# 39. Decisiones arquitectónicas

## ADR-001 — Separar Machine Learning de .NET

### Decisión

El Machine Learning se implementará como servicio independiente en Python.

### Motivo

Python proporciona un ecosistema especializado para:

* Data Science.
* Machine Learning.
* Experimentación.
* Procesamiento de datasets.

Mientras que .NET continuará siendo responsable del negocio y API principal.

---

## ADR-002 — Modular Monolith para backend

### Decisión

El backend de negocio permanecerá como una aplicación modular.

### Motivo

El proyecto no necesita convertir cada módulo en un microservicio independiente.

Esto reduce complejidad y permite concentrarse en:

* Arquitectura.
* Testing.
* Machine Learning.
* CI/CD.

---

## ADR-003 — Docker opcional

### Decisión

Docker no será requisito de desarrollo.

### Motivo

La máquina de desarrollo actual no permite utilizar virtualización.

La arquitectura deberá poder ejecutarse directamente sobre el sistema operativo.

---

## ADR-004 — Kubernetes opcional

### Decisión

Kubernetes será una fase posterior.

### Motivo

Kubernetes aporta valor como herramienta de aprendizaje de infraestructura, pero no es necesario para demostrar las capacidades principales del proyecto.

---

## ADR-005 — REST para comunicación

### Decisión

La comunicación inicial entre React, .NET y Python utilizará HTTP/REST.

### Motivo

REST permite mantener una arquitectura sencilla y fácil de depurar durante el aprendizaje.

No se utilizará gRPC o mensajería distribuida inicialmente.

---

# 40. Arquitectura final del MVP

```text
                         USER
                           │
                           ▼
                  ┌─────────────────┐
                  │      React      │
                  │   TypeScript    │
                  └────────┬────────┘
                           │
                           │ HTTPS
                           ▼
                  ┌─────────────────┐
                  │    .NET 10      │
                  │   ASP.NET API   │
                  │                 │
                  │ Clean Arch.     │
                  │ CQRS            │
                  │ JWT             │
                  └───────┬───┬─────┘
                          │   │
                  ┌───────┘   └──────────┐
                  │                      │
                  ▼                      ▼
          ┌───────────────┐       ┌───────────────┐
          │  PostgreSQL   │       │  Python API   │
          │               │       │    FastAPI    │
          │ Business Data │       │               │
          │ Predictions   │       │ Training      │
          │ Model Metadata│       │ Prediction    │
          └───────────────┘       └───────┬───────┘
                                          │
                                          ▼
                                  ┌───────────────┐
                                  │ ML Model      │
                                  │ scikit-learn  │
                                  └───────────────┘


                    ┌─────────────────────────┐
                    │     GitHub Actions      │
                    │                         │
                    │ Build / Test / Validate │
                    │ CI/CD                    │
                    └─────────────────────────┘
```

---

# 41. Criterio de finalización arquitectónica

La arquitectura del MVP se considerará implementada cuando:

* React consuma exclusivamente la API .NET.
* .NET implemente Clean Architecture.
* PostgreSQL almacene los datos del negocio.
* Python funcione como servicio independiente.
* .NET pueda solicitar predicciones al servicio Python.
* El modelo pueda entrenarse independientemente.
* Las predicciones puedan almacenarse.
* Exista versionado de modelos.
* Exista comparación entre modelos.
* Existan tests para los componentes principales.
* GitHub Actions ejecute los tests automáticamente.
* El proyecto pueda ejecutarse localmente sin Docker.
* Docker pueda agregarse posteriormente sin cambiar la lógica de negocio.
* Kubernetes pueda agregarse posteriormente como capa de infraestructura.

---

# 42. Principio arquitectónico principal

> **La arquitectura debe facilitar el aprendizaje sin introducir complejidad innecesaria.**

Smart Bakery no debe utilizar una tecnología únicamente para poder decir que fue utilizada.

Cada componente debe tener una responsabilidad clara:

```text
React
→ Presentación

.NET
→ Negocio

PostgreSQL
→ Datos

Python
→ Machine Learning

GitHub Actions
→ Automatización

Docker
→ Opcional: empaquetado

Kubernetes
→ Opcional: orquestación
```
