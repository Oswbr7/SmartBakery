# 🌐 Smart Bakery — API Specification

## 1. Objetivo

Este documento define los contratos HTTP de las APIs de Smart Bakery.

La arquitectura tendrá dos APIs principales:

```text
Frontend
   │
   ▼
.NET REST API
   │
   ├── PostgreSQL
   │
   └── Python ML API
```

### APIs

```text
.NET API
    ↓
Business API
    ↓
React

Python API
    ↓
Machine Learning API
    ↓
.NET API
```

El frontend **no debe comunicarse directamente con Python ni con PostgreSQL**.

---

# 2. API Architecture

```text
┌──────────────────────┐
│        React         │
│     TypeScript       │
└──────────┬───────────┘
           │
           │ HTTPS / REST
           ▼
┌──────────────────────┐
│      .NET API        │
│      ASP.NET Core    │
└───────┬────────┬─────┘
        │        │
        │        │ HTTP
        │        ▼
        │   ┌──────────────────────┐
        │   │    Python FastAPI    │
        │   │                      │
        │   │ Machine Learning     │
        │   └──────────┬───────────┘
        │              │
        │              ▼
        │       ┌──────────────┐
        │       │ Model Files  │
        │       └──────────────┘
        │
        ▼
┌──────────────────────┐
│      PostgreSQL      │
└──────────────────────┘
```

---

# 3. API Base URLs

## Development

.NET:

```text
https://localhost:5001/api
```

Python:

```text
http://localhost:8000
```

Los puertos pueden cambiar dependiendo de la configuración local.

---

# 4. API versioning

La API pública utilizará versionado:

```text
/api/v1
```

Ejemplo:

```text
GET /api/v1/products
```

La API de Machine Learning puede comenzar sin versionado público:

```text
/predict
/train
```

En una versión posterior podrá utilizar:

```text
/api/v1/predict
```

---

# 5. HTTP conventions

Utilizar:

```text
GET
POST
PUT
PATCH
DELETE
```

según corresponda.

### GET

Obtener información.

```text
GET /api/v1/products
```

### POST

Crear recursos o ejecutar operaciones.

```text
POST /api/v1/products
POST /api/v1/sales
POST /api/v1/predictions
```

### PUT

Actualizar un recurso completo.

### PATCH

Actualizar parcialmente un recurso.

### DELETE

Eliminar un recurso cuando la operación sea válida.

Para productos normalmente se preferirá:

```text
IsActive = false
```

en lugar de eliminar físicamente.

---

# 6. Authentication

La API .NET utilizará:

```text
JWT Bearer Authentication
```

Después del login:

```http
Authorization: Bearer <token>
```

---

# 7. Authentication endpoints

## POST /api/v1/auth/register

Crear un usuario.

### Request

```json
{
  "email": "admin@smartbakery.local",
  "password": "Password123!",
  "firstName": "Admin",
  "lastName": "User"
}
```

### Response

```http
201 Created
```

```json
{
  "id": "uuid",
  "email": "admin@smartbakery.local",
  "firstName": "Admin",
  "lastName": "User",
  "role": "Employee"
}
```

El rol inicial dependerá de las reglas de registro.

---

# 8. POST /api/v1/auth/login

Autenticar usuario.

### Request

```json
{
  "email": "admin@smartbakery.local",
  "password": "Password123!"
}
```

### Response

```http
200 OK
```

```json
{
  "accessToken": "jwt-token",
  "expiresAt": "2026-10-01T20:00:00Z",
  "user": {
    "id": "uuid",
    "email": "admin@smartbakery.local",
    "firstName": "Admin",
    "lastName": "User",
    "role": "Admin"
  }
}
```

---

# 9. GET /api/v1/auth/me

Obtener información del usuario autenticado.

### Response

```json
{
  "id": "uuid",
  "email": "admin@smartbakery.local",
  "firstName": "Admin",
  "lastName": "User",
  "role": "Admin"
}
```

---

# 10. Categories

## GET /api/v1/categories

Obtener categorías.

### Response

```json
[
  {
    "id": "uuid",
    "name": "Pasteles",
    "description": "Pasteles completos",
    "isActive": true
  },
  {
    "id": "uuid",
    "name": "Cheesecakes",
    "description": "Cheesecakes",
    "isActive": true
  }
]
```

---

# 11. GET /api/v1/categories/{id}

Obtener una categoría.

### Response

```json
{
  "id": "uuid",
  "name": "Pasteles",
  "description": "Pasteles completos",
  "isActive": true
}
```

---

# 12. POST /api/v1/categories

Crear categoría.

### Authorization

```text
Admin
```

### Request

```json
{
  "name": "Pasteles",
  "description": "Pasteles completos"
}
```

### Response

```http
201 Created
```

```json
{
  "id": "uuid",
  "name": "Pasteles",
  "description": "Pasteles completos",
  "isActive": true
}
```

---

# 13. PUT /api/v1/categories/{id}

Actualizar categoría.

### Request

```json
{
  "name": "Pasteles Premium",
  "description": "Pasteles premium"
}
```

### Response

```http
200 OK
```

---

# 14. PATCH /api/v1/categories/{id}/status

Activar o desactivar categoría.

### Request

```json
{
  "isActive": false
}
```

---

# 15. Products

## GET /api/v1/products

Obtener productos.

Soportará filtros y paginación.

Ejemplo:

```text
GET /api/v1/products?page=1&pageSize=20
```

Filtros:

```text
categoryId
isActive
search
```

Ejemplo:

```text
GET /api/v1/products?categoryId=uuid&isActive=true&search=chocolate
```

---

# 16. Product response

```json
{
  "items": [
    {
      "id": "uuid",
      "categoryId": "uuid",
      "categoryName": "Pasteles",
      "name": "Pastel de Chocolate",
      "description": "Pastel de chocolate",
      "price": 350.00,
      "isActive": true,
      "createdAt": "2026-09-01T15:00:00Z",
      "updatedAt": "2026-09-01T15:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 1,
  "totalPages": 1
}
```

---

# 17. GET /api/v1/products/{id}

Obtener producto.

### Response

```json
{
  "id": "uuid",
  "categoryId": "uuid",
  "categoryName": "Pasteles",
  "name": "Pastel de Chocolate",
  "description": "Pastel de chocolate",
  "price": 350.00,
  "isActive": true,
  "createdAt": "2026-09-01T15:00:00Z",
  "updatedAt": "2026-09-01T15:00:00Z"
}
```

---

# 18. POST /api/v1/products

Crear producto.

### Authorization

```text
Admin
```

### Request

```json
{
  "categoryId": "uuid",
  "name": "Pastel de Chocolate",
  "description": "Pastel de chocolate",
  "price": 350.00
}
```

### Response

```http
201 Created
```

```json
{
  "id": "uuid",
  "categoryId": "uuid",
  "name": "Pastel de Chocolate",
  "description": "Pastel de chocolate",
  "price": 350.00,
  "isActive": true
}
```

---

# 19. PUT /api/v1/products/{id}

Actualizar producto.

### Request

```json
{
  "categoryId": "uuid",
  "name": "Pastel de Chocolate Premium",
  "description": "Pastel premium de chocolate",
  "price": 380.00
}
```

---

# 20. PATCH /api/v1/products/{id}/status

Activar/desactivar producto.

### Request

```json
{
  "isActive": false
}
```

No se deberá eliminar físicamente un producto que tenga historial de ventas.

---

# 21. Promotions

## GET /api/v1/promotions

Obtener promociones.

Filtros:

```text
isActive
startDate
endDate
```

---

# 22. GET /api/v1/promotions/{id}

Obtener promoción.

---

# 23. POST /api/v1/promotions

Crear promoción.

### Authorization

```text
Admin
```

### Request

```json
{
  "name": "10% Weekend",
  "description": "Descuento durante el fin de semana",
  "discountType": "Percentage",
  "discountValue": 10,
  "startDate": "2026-10-01T00:00:00Z",
  "endDate": "2026-10-31T23:59:59Z"
}
```

---

# 24. PUT /api/v1/promotions/{id}

Actualizar promoción.

---

# 25. PATCH /api/v1/promotions/{id}/status

Activar/desactivar promoción.

---

# 26. Sales

## GET /api/v1/sales

Obtener historial de ventas.

Filtros:

```text
from
to
productId
createdByUserId
```

Ejemplo:

```text
GET /api/v1/sales?from=2026-09-01&to=2026-09-30
```

---

# 27. GET /api/v1/sales/{id}

Obtener detalle de venta.

### Response

```json
{
  "id": "uuid",
  "saleDate": "2026-10-01T16:30:00Z",
  "subtotal": 700.00,
  "discountAmount": 70.00,
  "totalAmount": 630.00,
  "promotionId": "uuid",
  "createdByUserId": "uuid",
  "items": [
    {
      "id": "uuid",
      "productId": "uuid",
      "productName": "Pastel de Chocolate",
      "quantity": 2,
      "unitPrice": 350.00,
      "discountAmount": 70.00,
      "subtotal": 630.00
    }
  ]
}
```

---

# 28. POST /api/v1/sales

Registrar una venta.

### Request

```json
{
  "saleDate": "2026-10-01T16:30:00Z",
  "promotionId": "uuid",
  "items": [
    {
      "productId": "uuid",
      "quantity": 2
    }
  ]
}
```

El backend será responsable de:

```text
Obtener precio actual
        ↓
Calcular subtotal
        ↓
Validar promoción
        ↓
Calcular descuento
        ↓
Calcular total
        ↓
Crear Sale
        ↓
Crear SaleItems
```

El frontend **no debe ser la fuente de verdad para los cálculos monetarios**.

---

# 29. Sale calculation

Ejemplo:

```text
Product:
$350

Quantity:
2

Subtotal:
$700

Discount:
10%

Discount amount:
$70

Total:
$630
```

El backend deberá realizar estos cálculos.

---

# 30. Dashboard

## GET /api/v1/dashboard/summary

Obtiene los principales KPIs.

### Response

```json
{
  "salesToday": 18,
  "revenueToday": 6250.00,
  "salesThisMonth": 342,
  "revenueThisMonth": 118500.00,
  "activeProducts": 24
}
```

---

# 31. GET /api/v1/dashboard/sales-over-time

Obtiene ventas agrupadas por fecha.

Parámetros:

```text
from
to
```

### Response

```json
[
  {
    "date": "2026-09-01",
    "quantity": 42,
    "revenue": 12400.00
  },
  {
    "date": "2026-09-02",
    "quantity": 38,
    "revenue": 11100.00
  }
]
```

---

# 32. GET /api/v1/dashboard/top-products

Obtiene productos con mayor cantidad vendida.

Parámetros:

```text
from
to
limit
```

### Response

```json
[
  {
    "productId": "uuid",
    "productName": "Pastel de Chocolate",
    "quantitySold": 124
  },
  {
    "productId": "uuid",
    "productName": "Cheesecake",
    "quantitySold": 97
  }
]
```

---

# 33. GET /api/v1/dashboard/sales-by-category

### Response

```json
[
  {
    "categoryId": "uuid",
    "categoryName": "Pasteles",
    "quantitySold": 320,
    "revenue": 102500.00
  }
]
```

---

# 34. Predictions

## GET /api/v1/predictions

Obtener predicciones históricas.

Filtros:

```text
productId
from
to
modelVersionId
```

---

# 35. GET /api/v1/predictions/{id}

Obtener una predicción específica.

### Response

```json
{
  "id": "uuid",
  "productId": "uuid",
  "productName": "Pastel de Chocolate",
  "predictionDate": "2026-10-15",
  "predictedQuantity": 20.4,
  "modelVersion": "v3",
  "createdAt": "2026-10-14T15:00:00Z"
}
```

---

# 36. POST /api/v1/predictions

Generar una predicción.

### Request

```json
{
  "productId": "uuid",
  "date": "2026-10-15"
}
```

### Flow

```text
React
  ↓
.NET
  ↓
Validate Product
  ↓
Get historical context
  ↓
Python ML API
  ↓
Prediction
  ↓
Save Prediction
  ↓
Return result
```

---

# 37. Prediction response

```json
{
  "id": "uuid",
  "productId": "uuid",
  "predictionDate": "2026-10-15",
  "predictedQuantity": 20.4,
  "modelVersion": "v3"
}
```

---

# 38. POST /api/v1/predictions/batch

Generar predicciones para múltiples productos.

### Request

```json
{
  "date": "2026-10-15",
  "productIds": [
    "uuid",
    "uuid",
    "uuid"
  ]
}
```

### Response

```json
{
  "date": "2026-10-15",
  "predictions": [
    {
      "productId": "uuid",
      "predictedQuantity": 20.4,
      "modelVersion": "v3"
    },
    {
      "productId": "uuid",
      "predictedQuantity": 14.2,
      "modelVersion": "v3"
    }
  ]
}
```

---

# 39. Model Versions

## GET /api/v1/models

Obtener modelos registrados.

### Response

```json
[
  {
    "id": "uuid",
    "version": "v3",
    "algorithm": "RandomForest",
    "mae": 3.91,
    "rmse": 5.42,
    "r2": 0.83,
    "datasetVersion": "sales_2026_09",
    "status": "Production",
    "trainingDate": "2026-10-01T10:00:00Z"
  }
]
```

---

# 40. GET /api/v1/models/{id}

Obtener detalles de un modelo.

---

# 41. GET /api/v1/models/production

Obtener el modelo actualmente en producción.

### Response

```json
{
  "id": "uuid",
  "version": "v3",
  "algorithm": "RandomForest",
  "mae": 3.91,
  "rmse": 5.42,
  "r2": 0.83,
  "status": "Production"
}
```

---

# 42. POST /api/v1/models/train

Solicitar entrenamiento.

### Authorization

```text
Admin
```

### Request

```json
{
  "datasetVersion": "sales_2026_10",
  "algorithm": "RandomForest"
}
```

### Flow

```text
.NET
  ↓
Python /train
  ↓
Dataset
  ↓
Feature Engineering
  ↓
Training
  ↓
Evaluation
  ↓
Model Artifact
  ↓
Metrics
  ↓
.NET
  ↓
ModelVersion
```

---

# 43. Training response

```json
{
  "modelVersion": "v4",
  "algorithm": "RandomForest",
  "mae": 3.72,
  "rmse": 5.11,
  "r2": 0.85,
  "status": "Candidate"
}
```

Un modelo recién entrenado será inicialmente:

```text
Candidate
```

No deberá convertirse automáticamente en producción sin pasar por el proceso de evaluación definido.

---

# 44. POST /api/v1/models/{id}/promote

Promover un modelo candidato a producción.

### Authorization

```text
Admin
```

### Preconditions

```text
Model exists
Model status = Candidate
Metrics available
Artifact valid
Evaluation successful
```

### Response

```json
{
  "modelVersion": "v4",
  "previousProductionVersion": "v3",
  "status": "Production"
}
```

El modelo anterior deberá pasar a:

```text
Retired
```

---

# 45. POST /api/v1/models/{id}/reject

Rechazar un modelo candidato.

### Authorization

```text
Admin
```

### Response

```json
{
  "modelVersion": "v4",
  "status": "Rejected"
}
```

---

# 46. Python ML API

La API Python será interna.

No deberá exponerse directamente al navegador.

```text
React
  X
  │
  └──── No direct access
             │
             ▼
        Python API
```

La comunicación será:

```text
.NET
 ↓
Python
```

---

# 47. GET /health

Endpoint de health check.

### Response

```json
{
  "status": "healthy"
}
```

---

# 48. GET /model

Obtener información del modelo cargado.

### Response

```json
{
  "version": "v3",
  "algorithm": "RandomForest",
  "status": "loaded"
}
```

---

# 49. POST /predict

Endpoint interno de predicción.

### Request

```json
{
  "productId": "uuid",
  "date": "2026-10-15"
}
```

La API deberá obtener o recibir las features necesarias.

---

# 50. Prediction response

```json
{
  "prediction": 20.4,
  "modelVersion": "v3"
}
```

---

# 51. POST /train

Endpoint interno de entrenamiento.

### Request

```json
{
  "datasetVersion": "sales_2026_10",
  "algorithm": "RandomForest"
}
```

### Response

```json
{
  "modelVersion": "v4",
  "algorithm": "RandomForest",
  "metrics": {
    "mae": 3.72,
    "rmse": 5.11,
    "r2": 0.85
  },
  "artifactPath": "models/model_v4.joblib"
}
```

---

# 52. GET /metrics

Obtener métricas del modelo cargado.

### Response

```json
{
  "modelVersion": "v3",
  "metrics": {
    "mae": 3.91,
    "rmse": 5.42,
    "r2": 0.83
  }
}
```

---

# 53. Error response

Todas las APIs deberán utilizar un formato consistente de errores.

Formato:

```json
{
  "type": "https://smart-bakery/errors/validation",
  "title": "Validation error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "price": [
      "Price must be greater than or equal to 0."
    ]
  },
  "traceId": "00-abc123"
}
```

El backend puede basarse en:

```text
RFC 7807 Problem Details
```

---

# 54. HTTP status codes

Utilizar códigos HTTP apropiados.

| Status | Uso                                         |
| ------ | ------------------------------------------- |
| `200`  | Operación exitosa                           |
| `201`  | Recurso creado                              |
| `204`  | Operación exitosa sin contenido             |
| `400`  | Request inválido                            |
| `401`  | No autenticado                              |
| `403`  | Sin permisos                                |
| `404`  | Recurso no encontrado                       |
| `409`  | Conflicto                                   |
| `422`  | Validación semántica, si se decide utilizar |
| `500`  | Error interno                               |
| `502`  | Error al comunicarse con ML service         |
| `503`  | Servicio temporalmente no disponible        |

---

# 55. Validation

La validación debe ocurrir principalmente en .NET.

Ejemplo:

```json
{
  "name": "",
  "price": -50
}
```

Respuesta:

```http
400 Bad Request
```

```json
{
  "title": "Validation error",
  "status": 400,
  "errors": {
    "name": [
      "Name is required."
    ],
    "price": [
      "Price must be greater than or equal to 0."
    ]
  }
}
```

---

# 56. Pagination

Las listas grandes deberán utilizar paginación.

Query:

```text
?page=1&pageSize=20
```

Response:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalItems": 100,
  "totalPages": 5
}
```

El `pageSize` deberá tener un límite máximo.

Ejemplo:

```text
Maximum pageSize = 100
```

---

# 57. Filtering

Las APIs deberán soportar filtros cuando tenga sentido.

Ejemplo:

```text
GET /api/v1/products?isActive=true
```

Ventas:

```text
GET /api/v1/sales?from=2026-09-01&to=2026-09-30
```

Predicciones:

```text
GET /api/v1/predictions?productId=uuid
```

---

# 58. Sorting

Cuando sea necesario:

```text
?sortBy=name
&sortDirection=asc
```

Ejemplo:

```text
GET /api/v1/products?sortBy=name&sortDirection=asc
```

Los campos permitidos deberán estar definidos explícitamente en backend para evitar consultas arbitrarias.

---

# 59. Date handling

Las fechas y timestamps deberán utilizar:

```text
ISO 8601
```

Ejemplo:

```text
2026-10-15T15:30:00Z
```

Para fechas sin hora:

```text
2026-10-15
```

Las fechas de negocio deben interpretarse teniendo en cuenta la zona horaria configurada para el negocio.

---

# 60. Money handling

Los valores monetarios se transmitirán como números decimales.

Ejemplo:

```json
{
  "price": 350.00
}
```

Nunca utilizar strings para representar dinero salvo que exista una razón específica de serialización.

---

# 61. API security

La API deberá implementar:

```text
JWT authentication
Role authorization
HTTPS
Input validation
Rate limiting where appropriate
CORS
Secure configuration
```

Nunca almacenar:

```text
Passwords
JWT secrets
Database credentials
API keys
```

directamente en el repositorio.

---

# 62. CORS

En desarrollo se permitirá el origen del frontend.

Ejemplo conceptual:

```text
https://localhost:5173
```

En producción se deberá utilizar únicamente el dominio autorizado.

No utilizar:

```text
AllowAnyOrigin()
```

como configuración permanente de producción.

---

# 63. ML API security

Aunque el ML service sea interno, no debe asumirse que una red interna equivale automáticamente a seguridad.

En una implementación posterior podrá utilizarse:

```text
Internal network
Service authentication
API key
JWT
mTLS
```

Para el MVP, la API Python puede permanecer accesible únicamente desde el backend.

---

# 64. Timeouts

El cliente .NET que consume Python debe tener un timeout.

Ejemplo conceptual:

```text
ML request timeout:
30 seconds
```

El valor final dependerá del tiempo esperado de inferencia y entrenamiento.

Las operaciones de entrenamiento pueden requerir un mecanismo asíncrono posteriormente.

---

# 65. Training operations

El entrenamiento de modelos puede ser una operación larga.

En el MVP:

```text
POST /models/train
```

puede ser síncrono.

En una arquitectura avanzada:

```text
POST /models/train
       ↓
202 Accepted
       ↓
Training Job
       ↓
Background Worker
       ↓
Completed
```

La implementación asíncrona no es necesaria para el MVP.

---

# 66. Idempotency

Las operaciones que creen recursos importantes deberán analizar si requieren idempotencia.

Por ejemplo:

```text
POST /sales
```

podría generar una venta duplicada si el cliente reintenta automáticamente la misma petición.

Una implementación futura puede utilizar:

```text
Idempotency-Key
```

Ejemplo:

```http
Idempotency-Key: abc123
```

No es obligatorio para el MVP.

---

# 67. Health checks

.NET:

```text
GET /health
```

Python:

```text
GET /health
```

El health check de .NET podrá comprobar:

```text
Application
Database
ML service
```

pero no necesariamente deberá considerar todos ellos como una única condición de "liveness".

Conceptualmente:

```text
/health/live
/health/ready
```

podrá introducirse posteriormente.

---

# 68. API observability

Las APIs deberán generar logs estructurados.

Registrar información como:

```text
Timestamp
Request
Endpoint
StatusCode
Duration
TraceId
UserId
```

No registrar:

```text
Passwords
JWT tokens
Sensitive credentials
```

---

# 69. Trace ID

Cada request deberá poder asociarse a un identificador.

Ejemplo:

```text
traceId:
00-abc123
```

Si ocurre un error:

```text
React
 ↓
.NET
 ↓
Python
```

el mismo contexto de trazabilidad podrá facilitar la investigación del problema.

---

# 70. API client in React

React tendrá una capa para consumir la API.

Ejemplo:

```text
src/
└── services/
    ├── api-client.ts
    ├── auth-api.ts
    ├── products-api.ts
    ├── sales-api.ts
    ├── dashboard-api.ts
    └── predictions-api.ts
```

No se deben colocar llamadas HTTP directamente dentro de componentes complejos.

---

# 71. API client in .NET

La comunicación con Python utilizará una abstracción:

```csharp
public interface IMachineLearningClient
{
    Task<PredictionResponse> PredictAsync(
        PredictionRequest request,
        CancellationToken cancellationToken);

    Task<TrainingResponse> TrainAsync(
        TrainingRequest request,
        CancellationToken cancellationToken);
}
```

La implementación:

```text
MachineLearningClient
```

utilizará:

```text
HttpClient
```

---

# 72. CQRS mapping

Los endpoints .NET se mapearán hacia Commands y Queries.

Ejemplo:

```text
POST /products
       ↓
CreateProductCommand
       ↓
CreateProductCommandHandler
```

Y:

```text
GET /products
       ↓
GetProductsQuery
       ↓
GetProductsQueryHandler
```

Ventas:

```text
POST /sales
       ↓
CreateSaleCommand
```

Predicción:

```text
POST /predictions
       ↓
CreatePredictionCommand
       ↓
IMachineLearningClient
```

---

# 73. API flow — Product

```text
React
  ↓
POST /api/v1/products
  ↓
ProductsController
  ↓
CreateProductCommand
  ↓
Handler
  ↓
Domain
  ↓
EF Core
  ↓
PostgreSQL
```

---

# 74. API flow — Sale

```text
React
  ↓
POST /api/v1/sales
  ↓
SalesController
  ↓
CreateSaleCommand
  ↓
Handler
  ↓
Validate products
  ↓
Calculate totals
  ↓
Save Sale
  ↓
Save SaleItems
  ↓
PostgreSQL
```

---

# 75. API flow — Prediction

```text
React
  ↓
POST /api/v1/predictions
  ↓
.NET API
  ↓
CreatePredictionCommand
  ↓
Get required data
  ↓
IMachineLearningClient
  ↓
Python /predict
  ↓
ML Model
  ↓
Prediction
  ↓
.NET
  ↓
PostgreSQL
  ↓
React
```

---

# 76. API flow — Training

```text
React / Admin
      ↓
POST /api/v1/models/train
      ↓
.NET
      ↓
Python /train
      ↓
Load dataset
      ↓
Feature Engineering
      ↓
Train
      ↓
Evaluate
      ↓
Serialize Model
      ↓
Return metrics
      ↓
.NET
      ↓
ModelVersion
      ↓
Candidate
```

---

# 77. API documentation

La API .NET deberá generar documentación utilizando:

```text
OpenAPI
Swagger
```

Durante desarrollo se podrá acceder a:

```text
/swagger
```

El contrato OpenAPI debe mantenerse actualizado junto con los endpoints.

---

# 78. API version evolution

Cuando una modificación rompa compatibilidad:

```text
/api/v1
```

podrá mantenerse mientras se desarrolla:

```text
/api/v2
```

Ejemplo:

```text
/api/v1/products
/api/v2/products
```

No se deberá romper silenciosamente el contrato existente.

---

# 79. MVP endpoints

El MVP debe implementar al menos:

### Authentication

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
GET  /api/v1/auth/me
```

### Categories

```text
GET  /api/v1/categories
POST /api/v1/categories
GET  /api/v1/categories/{id}
PUT  /api/v1/categories/{id}
```

### Products

```text
GET   /api/v1/products
POST  /api/v1/products
GET   /api/v1/products/{id}
PUT   /api/v1/products/{id}
PATCH /api/v1/products/{id}/status
```

### Sales

```text
GET  /api/v1/sales
GET  /api/v1/sales/{id}
POST /api/v1/sales
```

### Dashboard

```text
GET /api/v1/dashboard/summary
GET /api/v1/dashboard/sales-over-time
GET /api/v1/dashboard/top-products
```

### Predictions

```text
GET  /api/v1/predictions
POST /api/v1/predictions
POST /api/v1/predictions/batch
```

### Models

```text
GET  /api/v1/models
GET  /api/v1/models/{id}
GET  /api/v1/models/production
POST /api/v1/models/train
POST /api/v1/models/{id}/promote
POST /api/v1/models/{id}/reject
```

---

# 80. MVP Python endpoints

```text
GET  /health
GET  /model
GET  /metrics
POST /predict
POST /train
```

---

# 81. API Definition of Done

La API estará lista para el MVP cuando:

```text
[ ] Authentication implemented.
[ ] JWT authentication works.
[ ] Role authorization works.
[ ] Products endpoints work.
[ ] Categories endpoints work.
[ ] Sales endpoints work.
[ ] Promotions endpoints work.
[ ] Dashboard endpoints work.
[ ] Prediction endpoints work.
[ ] Model endpoints work.
[ ] Python ML API works.
[ ] .NET communicates with Python.
[ ] Validation implemented.
[ ] Consistent error responses implemented.
[ ] Pagination implemented where necessary.
[ ] Filtering implemented where necessary.
[ ] Swagger/OpenAPI available.
[ ] Health checks implemented.
[ ] Structured logging implemented.
[ ] No secrets stored in source code.
```

---

# 82. API Design Principles

Smart Bakery seguirá estos principios:

```text
1. React only communicates with .NET.
2. .NET owns business rules.
3. Python owns Machine Learning.
4. PostgreSQL owns persistence.
5. Controllers remain thin.
6. Application handlers coordinate use cases.
7. Domain contains business rules.
8. Infrastructure handles external dependencies.
9. API contracts should be explicit.
10. Errors should be predictable.
11. Authentication belongs to .NET.
12. ML should not directly manipulate the business database.
13. Long-running ML operations can become asynchronous later.
14. API versioning should protect compatibility.
15. Docker/Kubernetes must not change the API contract.
```

---

# 83. Final API architecture

```text
                       ┌───────────────────┐
                       │      React        │
                       └─────────┬─────────┘
                                 │
                           HTTPS / REST
                                 │
                                 ▼
                  ┌──────────────────────────┐
                  │        .NET API          │
                  │                          │
                  │ /auth                    │
                  │ /products                │
                  │ /categories              │
                  │ /sales                   │
                  │ /promotions              │
                  │ /dashboard               │
                  │ /predictions             │
                  │ /models                  │
                  └─────────┬───────┬────────┘
                            │       │
                       EF Core      │ HTTP
                            │       │
                            ▼       ▼
                    ┌──────────┐  ┌──────────────┐
                    │PostgreSQL│  │ Python ML    │
                    └──────────┘  │   FastAPI    │
                                  └──────┬───────┘
                                         │
                                         ▼
                                  ┌────────────┐
                                  │ ML Models  │
                                  │  .joblib   │
                                  └────────────┘
```

---

# 84. Core principle

> **The API is the contract between components. Business rules belong to .NET, Machine Learning belongs to Python, persistence belongs to PostgreSQL, and the frontend consumes the business API without needing to know how the backend is implemented.**


# Smart Bakery — API Authorization Rules

## 1. Authorization model

Smart Bakery utiliza autenticación mediante JWT y autorización basada en roles.

### Roles

```text
Admin
Employee
```

### Role hierarchy

```text
Admin
  │
  └── inherits Employee permissions
```

Por lo tanto:

```text
Admin
  ├── Can perform Employee operations
  └── Can perform Admin operations

Employee
  └── Can perform Employee operations
```

---

# 2. Authorization levels

| Level           | Authentication | Allowed roles   |
| --------------- | -------------- | --------------- |
| `Public`        | No             | Everyone        |
| `Authenticated` | Yes            | Admin, Employee |
| `Employee`      | Yes            | Employee, Admin |
| `Admin`         | Yes            | Admin only      |

---

# 3. Authentication endpoints

| Method | Endpoint                | Authorization     |
| ------ | ----------------------- | ----------------- |
| `POST` | `/api/v1/auth/register` | **Public**        |
| `POST` | `/api/v1/auth/login`    | **Public**        |
| `GET`  | `/api/v1/auth/me`       | **Authenticated** |

### Register

```text
POST /api/v1/auth/register
Authorization: Public
```

Any unauthenticated user can register.

However, the client must **never be allowed to choose `Admin` as part of public registration**.

Invalid:

```json
{
  "email": "user@example.com",
  "password": "Password123!",
  "role": "Admin"
}
```

The server must ignore or reject client-provided role escalation.

The initial role should be assigned by the backend.

---

### Login

```text
POST /api/v1/auth/login
Authorization: Public
```

Required because the user does not have a JWT yet.

---

### Current user

```text
GET /api/v1/auth/me
Authorization: Authenticated
```

Any authenticated user can retrieve their own profile.

A user cannot request another user's profile through this endpoint.

---

# 4. Categories

| Method  | Endpoint                         | Authorization |
| ------- | -------------------------------- | ------------- |
| `GET`   | `/api/v1/categories`             | **Employee**  |
| `GET`   | `/api/v1/categories/{id}`        | **Employee**  |
| `POST`  | `/api/v1/categories`             | **Admin**     |
| `PUT`   | `/api/v1/categories/{id}`        | **Admin**     |
| `PATCH` | `/api/v1/categories/{id}/status` | **Admin**     |

## GET categories

```text
Employee + Admin
```

Employees need categories to operate the product and sales interfaces.

## Create category

```text
Admin only
```

## Update category

```text
Admin only
```

## Change category status

```text
Admin only
```

Employees should not be able to activate/deactivate categories because this changes the catalog configuration.

---

# 5. Products

| Method  | Endpoint                       | Authorization |
| ------- | ------------------------------ | ------------- |
| `GET`   | `/api/v1/products`             | **Employee**  |
| `GET`   | `/api/v1/products/{id}`        | **Employee**  |
| `POST`  | `/api/v1/products`             | **Admin**     |
| `PUT`   | `/api/v1/products/{id}`        | **Admin**     |
| `PATCH` | `/api/v1/products/{id}/status` | **Admin**     |

## Read products

```text
Employee + Admin
```

Employees need access to products to register sales.

## Create product

```text
Admin only
```

## Update product

```text
Admin only
```

## Deactivate product

```text
Admin only
```

Products with historical sales must not be physically deleted.

---

# 6. Promotions

| Method  | Endpoint                         | Authorization |
| ------- | -------------------------------- | ------------- |
| `GET`   | `/api/v1/promotions`             | **Employee**  |
| `GET`   | `/api/v1/promotions/{id}`        | **Employee**  |
| `POST`  | `/api/v1/promotions`             | **Admin**     |
| `PUT`   | `/api/v1/promotions/{id}`        | **Admin**     |
| `PATCH` | `/api/v1/promotions/{id}/status` | **Admin**     |

## Read promotions

```text
Employee + Admin
```

Employees need to know which promotions are available when registering sales.

## Create/update promotion

```text
Admin only
```

Promotions directly affect pricing, so they are administrative operations.

## Activate/deactivate promotion

```text
Admin only
```

---

# 7. Sales

| Method | Endpoint             | Authorization |
| ------ | -------------------- | ------------- |
| `GET`  | `/api/v1/sales`      | **Employee**  |
| `GET`  | `/api/v1/sales/{id}` | **Employee**  |
| `POST` | `/api/v1/sales`      | **Employee**  |

## Read sales

```text
Employee + Admin
```

Employees can view sales history required for normal bakery operations.

## Create sale

```text
Employee + Admin
```

Employees are allowed to register sales.

The backend must calculate:

```text
Unit Price
Subtotal
Discount
Total
```

The client cannot override these values.

For example, the client must not be able to send:

```json
{
  "items": [
    {
      "productId": "uuid",
      "quantity": 2,
      "unitPrice": 1
    }
  ]
}
```

`unitPrice` should come from the backend/product database.

---

# 8. Sale ownership rules

Authorization is not limited to the role.

For future user-specific operations, Smart Bakery should distinguish:

```text
Role authorization
+
Resource authorization
```

For example:

```text
Employee
    ↓
Can view normal sales
```

But if a future endpoint exposes sensitive employee-specific information:

```text
Employee
    ↓
Only own resources

Admin
    ↓
All resources
```

This distinction should be preserved when additional endpoints are added.

---

# 9. Dashboard

| Method | Endpoint                              | Authorization |
| ------ | ------------------------------------- | ------------- |
| `GET`  | `/api/v1/dashboard/summary`           | **Employee**  |
| `GET`  | `/api/v1/dashboard/sales-over-time`   | **Employee**  |
| `GET`  | `/api/v1/dashboard/top-products`      | **Employee**  |
| `GET`  | `/api/v1/dashboard/sales-by-category` | **Employee**  |

The dashboard is operational information and can therefore be viewed by both roles.

```text
Employee + Admin
```

The backend must still apply authorization.

The frontend hiding a dashboard route is **not** considered security.

---

# 10. Predictions

| Method | Endpoint                    | Authorization |
| ------ | --------------------------- | ------------- |
| `GET`  | `/api/v1/predictions`       | **Employee**  |
| `GET`  | `/api/v1/predictions/{id}`  | **Employee**  |
| `POST` | `/api/v1/predictions`       | **Employee**  |
| `POST` | `/api/v1/predictions/batch` | **Employee**  |

Employees can consume predictions because predictions are part of the bakery's operational workflow.

Example:

```text
Tomorrow

Chocolate Cake
Predicted demand: 20

Cheesecake
Predicted demand: 14
```

---

# 11. Prediction security rules

The prediction endpoint must not allow the client to select an arbitrary model version.

For example, the client should **not** be allowed to submit:

```json
{
  "productId": "uuid",
  "date": "2026-10-15",
  "modelVersion": "v1"
}
```

and force the system to use `v1`.

Instead:

```text
POST /predictions
       ↓
.NET
       ↓
Production Model
       ↓
Python
```

The backend determines which model is currently in production.

This prevents normal users from bypassing the model lifecycle.

---

# 12. Models

Model management is an administrative capability.

| Method | Endpoint                      | Authorization |
| ------ | ----------------------------- | ------------- |
| `GET`  | `/api/v1/models`              | **Admin**     |
| `GET`  | `/api/v1/models/{id}`         | **Admin**     |
| `GET`  | `/api/v1/models/production`   | **Admin**     |
| `POST` | `/api/v1/models/train`        | **Admin**     |
| `POST` | `/api/v1/models/{id}/promote` | **Admin**     |
| `POST` | `/api/v1/models/{id}/reject`  | **Admin**     |

---

# 13. Why model endpoints are Admin-only

Model operations can have significant system-wide effects.

For example:

```text
Train
   ↓
Candidate
   ↓
Evaluate
   ↓
Promote
   ↓
Production
```

Promoting a model changes predictions for the entire application.

Therefore:

```text
Employee
    X
    │
    └── Cannot train/promote/reject models

Admin
    ✓
    └── Can manage model lifecycle
```

---

# 14. Training authorization

```text
POST /api/v1/models/train
Authorization: Admin
```

Training is an administrative operation.

The backend should also validate:

```text
User authenticated
        ↓
Role = Admin
        ↓
Dataset exists
        ↓
Dataset is valid
        ↓
Training allowed
```

The frontend should not be trusted to enforce this.

---

# 15. Model promotion authorization

```text
POST /api/v1/models/{id}/promote
Authorization: Admin
```

Additional business rules:

```text
Model exists
      ↓
Model = Candidate
      ↓
Metrics available
      ↓
Artifact exists
      ↓
Evaluation passed
      ↓
Admin promotes model
```

The endpoint must not allow:

```text
Rejected → Production
Retired → Production
```

unless a deliberate future workflow explicitly permits it.

---

# 16. Model rejection authorization

```text
POST /api/v1/models/{id}/reject
Authorization: Admin
```

Only candidate models should normally be rejectable.

```text
Candidate → Rejected
```

Invalid transitions:

```text
Production → Rejected
Retired → Rejected
```

The model lifecycle must be enforced server-side.

---

# 17. Python ML API

The Python API is an **internal service**.

It should not be publicly exposed to the browser.

| Method | Endpoint   | Caller                |
| ------ | ---------- | --------------------- |
| `GET`  | `/health`  | Infrastructure / .NET |
| `GET`  | `/model`   | .NET                  |
| `GET`  | `/metrics` | .NET                  |
| `POST` | `/predict` | .NET                  |
| `POST` | `/train`   | .NET                  |

---

# 18. Python `/health`

```text
GET /health
```

Authorization:

```text
Internal network / health-check access
```

The endpoint may remain unauthenticated if it is only exposed on a private network and contains no sensitive information.

It should return only health information.

Example:

```json
{
  "status": "healthy"
}
```

It must not return:

```text
API keys
Database credentials
Environment variables
Model secrets
```

---

# 19. Python `/model`

```text
GET /model
```

Caller:

```text
.NET ML client
```

Authorization:

```text
Internal service authentication
```

The endpoint should not be exposed publicly.

---

# 20. Python `/metrics`

```text
GET /metrics
```

Caller:

```text
.NET
```

Authorization:

```text
Internal service authentication
```

It may expose:

```text
MAE
RMSE
R²
Model version
```

but should not expose sensitive infrastructure configuration.

---

# 21. Python `/predict`

```text
POST /predict
```

Caller:

```text
.NET API
```

Authorization:

```text
Internal service authentication
```

Expected flow:

```text
React
   ↓
.NET
   ↓
Authenticated internal request
   ↓
Python
   ↓
Model
```

The browser must never call:

```text
POST http://ml-service:8000/predict
```

directly.

---

# 22. Python `/train`

```text
POST /train
```

Caller:

```text
.NET API
```

Authorization:

```text
Internal service authentication
```

The Python API should not independently decide which human user is allowed to train models.

That business authorization belongs to .NET.

The flow is:

```text
Admin
  ↓
.NET JWT authorization
  ↓
.NET verifies Admin role
  ↓
.NET calls Python
  ↓
Python performs training
```

---

# 23. Separation of responsibilities

The authorization boundary is:

```text
                  ┌────────────────────────┐
                  │       .NET API         │
                  │                        │
                  │ Authentication         │
                  │ Authorization          │
                  │ Business rules         │
                  │ User roles              │
                  └───────────┬────────────┘
                              │
                              │ trusted
                              │ internal call
                              ▼
                  ┌────────────────────────┐
                  │      Python ML API     │
                  │                        │
                  │ Training               │
                  │ Prediction             │
                  │ Evaluation             │
                  │ Model loading           │
                  └────────────────────────┘
```

Python does not need to know:

```text
Admin
Employee
JWT
React
```

for the MVP.

---

# 24. Global authorization policy

All `.NET` endpoints should be protected by default.

Conceptually:

```text
Default policy:
Authenticated
```

Public endpoints must explicitly opt out.

Therefore, a newly created endpoint should not accidentally become public.

---

# 25. Public endpoint allowlist

The only public endpoints in the MVP are:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
GET  /health
```

Potentially:

```text
GET /swagger
```

may be available in Development only.

Swagger should not automatically be publicly exposed in Production.

---

# 26. Admin policy

The backend should define a reusable authorization policy.

Conceptually:

```csharp
[Authorize(Policy = "AdminOnly")]
```

or:

```csharp
[Authorize(Roles = "Admin")]
```

The exact implementation can be decided during backend development.

The important requirement is that authorization is enforced server-side.

---

# 27. Employee policy

Employee-accessible endpoints can use:

```csharp
[Authorize(Roles = "Employee,Admin")]
```

or a dedicated policy:

```csharp
[Authorize(Policy = "EmployeeAccess")]
```

The dedicated policy is preferable if authorization becomes more complex later.

---

# 28. Authorization matrix

## Complete MVP matrix

| Resource    | Endpoint                           | Public | Employee | Admin |
| ----------- | ---------------------------------- | :----: | :------: | :---: |
| Auth        | `POST /auth/register`              |    ✓   |     ✓    |   ✓   |
| Auth        | `POST /auth/login`                 |    ✓   |     ✓    |   ✓   |
| Auth        | `GET /auth/me`                     |    —   |     ✓    |   ✓   |
| Categories  | `GET /categories`                  |    —   |     ✓    |   ✓   |
| Categories  | `GET /categories/{id}`             |    —   |     ✓    |   ✓   |
| Categories  | `POST /categories`                 |    —   |     —    |   ✓   |
| Categories  | `PUT /categories/{id}`             |    —   |     —    |   ✓   |
| Categories  | `PATCH /categories/{id}/status`    |    —   |     —    |   ✓   |
| Products    | `GET /products`                    |    —   |     ✓    |   ✓   |
| Products    | `GET /products/{id}`               |    —   |     ✓    |   ✓   |
| Products    | `POST /products`                   |    —   |     —    |   ✓   |
| Products    | `PUT /products/{id}`               |    —   |     —    |   ✓   |
| Products    | `PATCH /products/{id}/status`      |    —   |     —    |   ✓   |
| Promotions  | `GET /promotions`                  |    —   |     ✓    |   ✓   |
| Promotions  | `GET /promotions/{id}`             |    —   |     ✓    |   ✓   |
| Promotions  | `POST /promotions`                 |    —   |     —    |   ✓   |
| Promotions  | `PUT /promotions/{id}`             |    —   |     —    |   ✓   |
| Promotions  | `PATCH /promotions/{id}/status`    |    —   |     —    |   ✓   |
| Sales       | `GET /sales`                       |    —   |     ✓    |   ✓   |
| Sales       | `GET /sales/{id}`                  |    —   |     ✓    |   ✓   |
| Sales       | `POST /sales`                      |    —   |     ✓    |   ✓   |
| Dashboard   | `GET /dashboard/summary`           |    —   |     ✓    |   ✓   |
| Dashboard   | `GET /dashboard/sales-over-time`   |    —   |     ✓    |   ✓   |
| Dashboard   | `GET /dashboard/top-products`      |    —   |     ✓    |   ✓   |
| Dashboard   | `GET /dashboard/sales-by-category` |    —   |     ✓    |   ✓   |
| Predictions | `GET /predictions`                 |    —   |     ✓    |   ✓   |
| Predictions | `GET /predictions/{id}`            |    —   |     ✓    |   ✓   |
| Predictions | `POST /predictions`                |    —   |     ✓    |   ✓   |
| Predictions | `POST /predictions/batch`          |    —   |     ✓    |   ✓   |
| Models      | `GET /models`                      |    —   |     —    |   ✓   |
| Models      | `GET /models/{id}`                 |    —   |     —    |   ✓   |
| Models      | `GET /models/production`           |    —   |     —    |   ✓   |
| Models      | `POST /models/train`               |    —   |     —    |   ✓   |
| Models      | `POST /models/{id}/promote`        |    —   |     —    |   ✓   |
| Models      | `POST /models/{id}/reject`         |    —   |     —    |   ✓   |

---

# 29. HTTP authorization responses

The API must distinguish authentication failures from authorization failures.

### No JWT

```text
401 Unauthorized
```

Example:

```text
Employee
  ↓
GET /products
  ↓
No token
  ↓
401
```

### Valid JWT but insufficient role

```text
403 Forbidden
```

Example:

```text
Employee
  ↓
POST /products
  ↓
JWT valid
  ↓
Admin required
  ↓
403
```

---

# 30. Frontend authorization

React may hide UI elements based on the user's role.

For example:

```text
Admin:
  Products → Edit
  Products → Delete/Deactivate
  Models → Train
  Models → Promote

Employee:
  Products → View
  Sales → Create
  Predictions → View
```

However:

> **Frontend authorization is a UX feature, not a security boundary.**

Even if a button is hidden, the backend must reject unauthorized requests.

---

# 31. Example frontend behavior

Admin:

```text
Dashboard
Products
Sales
Predictions
Model Management
```

Employee:

```text
Dashboard
Products
Sales
Predictions
```

The Employee should not see:

```text
Model Management
```

but even if an Employee manually calls:

```text
POST /api/v1/models/train
```

the API must return:

```text
403 Forbidden
```

---

# 32. Authorization tests

Every protected endpoint must have authorization tests.

At minimum:

```text
Unauthenticated
      ↓
401

Employee
      ↓
Employee endpoint
      ↓
200/201/etc.

Employee
      ↓
Admin endpoint
      ↓
403

Admin
      ↓
Employee endpoint
      ↓
200/201/etc.

Admin
      ↓
Admin endpoint
      ↓
200/201/etc.
```

---

# 33. Minimum authorization test matrix

For every Admin endpoint:

```text
[ ] Anonymous → 401
[ ] Employee → 403
[ ] Admin → success
```

For every Employee endpoint:

```text
[ ] Anonymous → 401
[ ] Employee → success
[ ] Admin → success
```

For every Public endpoint:

```text
[ ] Anonymous → success
```

---

# 34. Business authorization vs role authorization

Role authorization answers:

```text
"Can this type of user access this endpoint?"
```

Business authorization answers:

```text
"Can this user perform this operation on this particular resource?"
```

Both may eventually be necessary.

Example:

```text
Admin
  ↓
Can manage models

Employee
  ↓
Can create sales
```

But a future feature might require:

```text
Employee
  ↓
Can only edit their own draft sale
```

That is a resource/business authorization rule rather than simply a role rule.

---

# 35. Future roles

The initial system should only implement:

```text
Admin
Employee
```

Potential future roles:

```text
Manager
Baker
Cashier
Analyst
```

These should not be implemented until there is an actual requirement.

Avoid creating a complex permission system prematurely.

---

# 36. Authorization Definition of Done

Authorization is complete when:

```text
[ ] All .NET endpoints have an explicit authorization rule.
[ ] Public endpoints are explicitly identified.
[ ] Default .NET policy requires authentication.
[ ] Admin endpoints reject Employees.
[ ] Employee endpoints accept Employees and Admins.
[ ] Unauthorized requests return 401.
[ ] Forbidden requests return 403.
[ ] JWT roles are validated server-side.
[ ] Client-provided roles cannot escalate privileges.
[ ] Model lifecycle operations are Admin-only.
[ ] Python ML API is not directly accessible by React.
[ ] Python service communication is internal.
[ ] Frontend hides unauthorized UI but does not act as the security boundary.
[ ] Authorization integration tests exist.
```

---

# 37. Final authorization architecture

```text
                         ┌──────────────┐
                         │    React     │
                         └──────┬───────┘
                                │
                                │ JWT
                                ▼
                    ┌─────────────────────┐
                    │      .NET API       │
                    │                     │
                    │ Authentication      │
                    │ Authorization       │
                    │ Business Rules      │
                    └─────────┬───────────┘
                              │
               ┌──────────────┴──────────────┐
               │                             │
               ▼                             ▼
       ┌───────────────┐             ┌────────────────┐
       │  PostgreSQL   │             │  Python ML API │
       │               │             │                │
       │ Business Data │             │ Predict        │
       │ Sales         │             │ Train          │
       │ Predictions   │             │ Evaluate       │
       │ Models        │             │ Models         │
       └───────────────┘             └────────────────┘
```

The core security principle is:

```text
User
 ↓
JWT
 ↓
.NET Authentication
 ↓
.NET Authorization
 ↓
Business Operation
 ↓
Database / ML Service
```

**Nunca confiar en el frontend para decidir qué puede hacer un usuario.**
