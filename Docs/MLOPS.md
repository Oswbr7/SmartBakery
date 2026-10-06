# 🧠 Smart Bakery — MLOps Specification

## 1. Objetivo

Este documento define la estrategia de **Machine Learning Operations (MLOps)** de Smart Bakery.

El objetivo es establecer un proceso reproducible para:

```text
Obtener datos
    ↓
Preparar dataset
    ↓
Feature Engineering
    ↓
Entrenar modelo
    ↓
Evaluar modelo
    ↓
Registrar versión
    ↓
Comparar con producción
    ↓
Promover o rechazar
    ↓
Realizar predicciones
    ↓
Monitorear resultados
    ↓
Reentrenar
```

El sistema deberá permitir evolucionar desde un modelo entrenado manualmente hasta un proceso de entrenamiento y despliegue automatizado.

---

# 2. ¿Qué significa MLOps en Smart Bakery?

MLOps será la combinación de:

```text
Machine Learning
        +
Software Engineering
        +
CI/CD
        +
Model Versioning
        +
Data Management
        +
Monitoring
        +
Automation
```

El objetivo no es solamente conseguir un modelo con buena precisión.

El sistema debe permitir responder preguntas como:

* ¿Qué modelo está actualmente en producción?
* ¿Con qué dataset fue entrenado?
* ¿Qué algoritmo utilizó?
* ¿Cuándo fue entrenado?
* ¿Qué métricas obtuvo?
* ¿Qué versión de features utilizó?
* ¿Es mejor que el modelo actual?
* ¿Por qué fue rechazado un modelo?
* ¿Podemos volver a una versión anterior?
* ¿Cuándo debemos volver a entrenar?
* ¿Qué tan bien están funcionando las predicciones actuales?

---

# 3. MLOps Architecture

La arquitectura será:

```text
                       ┌───────────────────┐
                       │   Historical Data │
                       │    PostgreSQL     │
                       └─────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │ Dataset Creation  │
                       └─────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │ Feature Engineering│
                       └─────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │ Model Training    │
                       └─────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │ Model Evaluation  │
                       └─────────┬─────────┘
                                 │
                                 ▼
                       ┌───────────────────┐
                       │ Model Registry    │
                       └─────────┬─────────┘
                                 │
                       ┌─────────┴─────────┐
                       │                   │
                       ▼                   ▼
                  Candidate             Production
                       │                   │
                       │                   ▼
                       │             Predictions
                       │                   │
                       │                   ▼
                       │              Monitoring
                       │                   │
                       └──────────┬────────┘
                                  │
                                  ▼
                             Retraining
```

---

# 4. MLOps Components

Smart Bakery tendrá los siguientes componentes:

```text
Dataset
Feature Pipeline
Training Pipeline
Evaluation Pipeline
Model Registry
Model Storage
Prediction Service
Monitoring
Retraining Process
CI/CD
```

---

# 5. MLOps Ownership

Cada componente debe tener una responsabilidad clara.

| Componente          | Responsabilidad                  |
| ------------------- | -------------------------------- |
| PostgreSQL          | Datos históricos del negocio     |
| .NET API            | Reglas de negocio y orquestación |
| Python ML Service   | Machine Learning                 |
| Dataset Pipeline    | Construcción del dataset         |
| Feature Engineering | Transformación de datos          |
| Training Pipeline   | Entrenamiento                    |
| Evaluation Pipeline | Evaluación                       |
| Model Registry      | Metadata y versiones             |
| Model Storage       | Archivos de modelos              |
| Prediction Service  | Inferencia                       |
| Monitoring          | Seguimiento del modelo           |
| GitHub Actions      | Automatización CI/CD             |

---

# 6. Model Lifecycle

Cada modelo deberá seguir un ciclo de vida.

```text
Training
   ↓
Candidate
   ↓
Evaluation
   ↓
┌───────────────┐
│               │
▼               ▼
Approved      Rejected
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

---

# 7. Candidate

Un modelo se convierte en `Candidate` después de completar correctamente el entrenamiento.

Ejemplo:

```text
Model Version: v4
Algorithm: RandomForest
Status: Candidate
MAE: 3.72
RMSE: 5.11
R²: 0.85
```

Un candidato todavía no debe utilizarse para predicciones de producción.

---

# 8. Production

Un modelo pasa a `Production` cuando cumple las condiciones necesarias para ser promovido.

Debe:

```text
Existir
Tener artifact válido
Tener métricas
Haber completado evaluación
Cumplir los criterios definidos
```

Solamente un modelo debe estar marcado como:

```text
Production
```

para una determinada estrategia de predicción.

---

# 9. Rejected

Un modelo puede ser rechazado cuando:

```text
Tiene métricas insuficientes
Es peor que el modelo actual
Tiene problemas durante evaluación
El artifact es inválido
El dataset presenta problemas
```

Un modelo rechazado no debe utilizarse para producción.

---

# 10. Retired

Cuando un modelo nuevo es promovido:

```text
Production v3
       ↓
Production v4
       ↓
v3 = Retired
v4 = Production
```

El modelo retirado debe conservarse para:

```text
Auditoría
Comparación
Reproducción
Rollback
Análisis histórico
```

No debe eliminarse automáticamente.

---

# 11. Model Versioning

Cada modelo debe tener una versión única.

Ejemplo:

```text
v1
v2
v3
v4
```

La versión debe estar asociada a:

```text
Algorithm
DatasetVersion
FeatureVersion
TrainingDate
Metrics
Artifact
Status
```

Ejemplo:

```json
{
  "version": "v4",
  "algorithm": "RandomForest",
  "datasetVersion": "sales_2026_10",
  "featureVersion": "features_v2",
  "mae": 3.72,
  "rmse": 5.11,
  "r2": 0.85,
  "status": "Candidate"
}
```

---

# 12. Dataset Versioning

No solamente se deben versionar los modelos.

También debe conocerse qué datos fueron utilizados.

Ejemplo:

```text
Dataset v1
sales_2026_01

Dataset v2
sales_2026_03

Dataset v3
sales_2026_06

Dataset v4
sales_2026_10
```

Cada modelo deberá indicar:

```text
DatasetVersion
```

Esto permite reproducir aproximadamente el entrenamiento.

---

# 13. Feature Versioning

Las features también pueden cambiar.

Ejemplo:

```text
features_v1
```

contiene:

```text
day_of_week
month
price
previous_day_sales
previous_7_days_average
```

Posteriormente:

```text
features_v2
```

puede agregar:

```text
promotion
holiday
previous_30_days_average
season
```

El modelo debe almacenar qué versión de features utilizó.

---

# 14. Reproducibility

El entrenamiento debe intentar ser reproducible.

Se deberán registrar:

```text
Dataset version
Feature version
Algorithm
Hyperparameters
Random seed
Training date
Python version
Library versions
Metrics
Model artifact
```

Ejemplo:

```json
{
  "algorithm": "RandomForest",
  "randomSeed": 42,
  "nEstimators": 200,
  "maxDepth": 12,
  "pythonVersion": "3.12",
  "datasetVersion": "sales_2026_10",
  "featureVersion": "features_v2"
}
```

La reproducibilidad exacta puede variar dependiendo de librerías y entorno, pero el objetivo es conservar suficiente información para reconstruir el entrenamiento.

---

# 15. Model Artifact

El modelo entrenado será serializado.

Para el MVP:

```text
.joblib
```

Ejemplo:

```text
models/
├── model_v1.joblib
├── model_v2.joblib
├── model_v3.joblib
└── model_v4.joblib
```

El artifact no debe almacenarse directamente en PostgreSQL.

PostgreSQL deberá almacenar metadata:

```text
ModelVersion
Algorithm
Metrics
ArtifactPath
```

---

# 16. Model Storage

Durante desarrollo:

```text
ml-service/models/
```

Para una arquitectura futura:

```text
Amazon S3
Azure Blob Storage
MinIO
```

La aplicación deberá abstraer el almacenamiento cuando sea posible.

Conceptualmente:

```text
IModelStorage
      │
      ├── LocalModelStorage
      │
      ├── S3ModelStorage
      │
      └── AzureBlobModelStorage
```

El MVP utilizará:

```text
LocalModelStorage
```

---

# 17. Model Registry

El Model Registry será responsable de almacenar información sobre los modelos.

Para el MVP podrá implementarse mediante PostgreSQL.

Entidad conceptual:

```text
ModelVersion
```

Campos principales:

```text
Id
Version
Algorithm
DatasetVersion
FeatureVersion
MAE
RMSE
R2
ArtifactPath
Status
TrainingDate
CreatedAt
```

Opcionalmente:

```text
Hyperparameters
TrainingDuration
PythonVersion
LibraryVersions
Notes
RejectedReason
```

---

# 18. Model Registry Example

```text
┌─────────────────────────────────────┐
│ ModelVersion                        │
├─────────────────────────────────────┤
│ v1                                  │
│ LinearRegression                    │
│ MAE: 5.20                           │
│ Status: Retired                     │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ ModelVersion                        │
├─────────────────────────────────────┤
│ v2                                  │
│ RandomForest                        │
│ MAE: 4.10                           │
│ Status: Retired                     │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ ModelVersion                        │
├─────────────────────────────────────┤
│ v3                                  │
│ GradientBoosting                    │
│ MAE: 3.91                           │
│ Status: Production                  │
└─────────────────────────────────────┘
```

---

# 19. Training Pipeline

El pipeline de entrenamiento será:

```text
1. Load Data
       ↓
2. Validate Data
       ↓
3. Clean Data
       ↓
4. Feature Engineering
       ↓
5. Split Dataset
       ↓
6. Train Model
       ↓
7. Evaluate Model
       ↓
8. Serialize Artifact
       ↓
9. Register Model
       ↓
10. Candidate
```

---

# 20. Data Validation

Antes del entrenamiento se deben comprobar los datos.

Validaciones:

```text
Dataset no vacío
Fechas válidas
ProductId válido
Quantity >= 0
Price >= 0
No columnas obligatorias faltantes
No tipos inválidos
No duplicados inesperados
```

Si la validación falla:

```text
Training = Failed
```

El modelo no deberá registrarse como candidato válido.

---

# 21. Data Quality

Se deben controlar problemas como:

```text
Missing values
Duplicated records
Invalid dates
Negative quantities
Invalid prices
Outliers extremos
Productos inexistentes
Datos incompletos
```

No todos los outliers deben eliminarse automáticamente.

Un valor extremo puede representar una venta real.

---

# 22. Feature Engineering

Las features estarán orientadas a demanda.

Ejemplos:

```text
day_of_week
day_of_month
month
week_of_year
is_weekend
season
price
promotion
previous_day_sales
previous_7_days_average
previous_30_days_average
```

El proceso debe ser consistente entre:

```text
Training
Prediction
```

La misma lógica de generación de features debe utilizarse en ambos casos.

---

# 23. Training Dataset Split

Para series temporales no se deberá utilizar aleatoriamente un split tradicional como única estrategia.

Se deberá respetar el orden temporal.

Ejemplo:

```text
Historical Data
──────────────────────────────────────────>

Train                 Validation       Test
───────────────────   ───────────      ───────
80%                    10%              10%
```

Ejemplo:

```text
January ───────── August
                         September
                                  October
```

El modelo debe entrenarse utilizando datos del pasado para predecir datos posteriores.

---

# 24. Baseline Model

Antes de utilizar modelos complejos debe existir un baseline.

Ejemplo:

```text
7-day moving average
```

Predicción:

```text
Prediction =
average sales of previous 7 days
```

El modelo ML deberá compararse contra este baseline.

Esto permite determinar si Machine Learning realmente aporta valor.

---

# 25. Candidate Evaluation

Después del entrenamiento:

```text
Candidate
   ↓
Evaluate
   ↓
Compare with Production
```

Las métricas principales serán:

```text
MAE
RMSE
R²
```

La métrica principal será:

```text
MAE
```

porque representa el error promedio absoluto en unidades vendidas.

---

# 26. Model Comparison

Ejemplo:

```text
Production Model

MAE = 4.10
```

Nuevo candidato:

```text
Candidate

MAE = 3.72
```

El pipeline deberá comparar ambos modelos bajo condiciones equivalentes.

No se debe comparar únicamente una métrica obtenida sobre datasets diferentes.

---

# 27. Promotion Criteria

El sistema debe definir criterios explícitos para promoción.

Ejemplo conceptual:

```text
Candidate MAE < Production MAE
```

y:

```text
Candidate RMSE <= acceptable threshold
```

y:

```text
Training completed successfully
```

y:

```text
Artifact valid
```

Los criterios exactos podrán cambiar a medida que el proyecto evolucione.

---

# 28. Model Promotion Flow

```text
Candidate
    ↓
Validate Artifact
    ↓
Validate Metrics
    ↓
Compare with Production
    ↓
┌───────────────┐
│               │
▼               ▼
Promote       Reject
│               │
▼               ▼
Production    Rejected
│
▼
Previous Production
→ Retired
```

---

# 29. Manual vs Automatic Promotion

Durante el MVP:

```text
Training
   ↓
Candidate
   ↓
Evaluation
   ↓
Admin
   ↓
Promote
```

La promoción será manual.

Posteriormente:

```text
Training
   ↓
Evaluation
   ↓
Automatic Gate
   ↓
Promote
```

podrá implementarse en CI/CD.

Esto permite comenzar con una arquitectura sencilla y agregar automatización progresivamente.

---

# 30. Model Rollback

Debe ser posible regresar a un modelo anterior.

Ejemplo:

```text
v3 = Production
v4 = Candidate
```

Después:

```text
v4 = Production
v3 = Retired
```

Si posteriormente se detecta un problema:

```text
Rollback
   ↓
v3 = Production
v4 = Retired
```

El rollback debe utilizar un artifact previamente registrado.

No se debe volver a entrenar para realizar un rollback.

---

# 31. Prediction Lifecycle

Una predicción seguirá:

```text
Request
   ↓
Validate
   ↓
Get Production Model
   ↓
Generate Features
   ↓
ML Prediction
   ↓
Persist Prediction
   ↓
Return Response
```

Cada predicción debe conservar:

```text
Product
PredictionDate
PredictedQuantity
ModelVersion
CreatedAt
```

Esto permite conocer qué modelo produjo cada resultado.

---

# 32. Prediction Traceability

Ejemplo:

```text
Prediction
──────────────────────────
Product:
Chocolate Cake

Date:
2026-10-15

Prediction:
20.4 units

Model:
v3

Algorithm:
RandomForest

Created:
2026-10-14
```

Esto permite responder:

> ¿Qué modelo generó esta predicción?

---

# 33. Actual vs Predicted

Una vez que la fecha de la predicción haya pasado, puede compararse:

```text
Predicted:
20

Actual:
24
```

Error:

```text
|24 - 20| = 4
```

Esto permite comenzar a medir el desempeño real del modelo después del despliegue.

---

# 34. Production Monitoring

El sistema deberá monitorear progresivamente:

```text
Prediction volume
Prediction errors
Model version
Actual vs predicted
Data quality
Feature distribution
Model performance
```

---

# 35. Model Performance Monitoring

Una vez disponibles las ventas reales:

```text
Prediction
      ↓
Wait for actual sales
      ↓
Compare
      ↓
Calculate error
```

Ejemplo:

```text
Predicted = 20
Actual    = 24
MAE       = 4
```

Los errores podrán agregarse por:

```text
Day
Week
Month
Product
Category
Model Version
```

---

# 36. Performance History

El sistema podrá almacenar métricas reales de producción.

Ejemplo:

```text
Model v3

September:
MAE = 3.9

October:
MAE = 4.2

November:
MAE = 5.1
```

Esto permite detectar degradación del modelo.

---

# 37. Model Drift

El sistema deberá considerar dos tipos de drift.

### Data Drift

Cambios en la distribución de los datos.

Ejemplo:

```text
Average price
Average quantity
Weekend sales
Promotion frequency
```

cambian significativamente.

### Concept Drift

La relación entre las features y la demanda cambia.

Ejemplo:

```text
Una promoción que anteriormente aumentaba
las ventas un 20% deja de producir el mismo efecto.
```

No es necesario implementar detección avanzada de drift en el MVP.

Debe quedar contemplado arquitectónicamente.

---

# 38. Drift Monitoring — Future

Una implementación futura puede calcular:

```text
PSI
KL Divergence
Distribution comparison
Feature statistics
```

No se debe agregar complejidad de este tipo antes de tener suficiente información histórica.

---

# 39. Retraining

El modelo podrá reentrenarse periódicamente.

Ejemplo:

```text
New Sales Data
      ↓
Dataset Updated
      ↓
Retraining
      ↓
Evaluation
      ↓
Candidate
      ↓
Promotion
```

El reentrenamiento no significa necesariamente:

```text
Always replace Production
```

Cada modelo nuevo debe pasar por evaluación.

---

# 40. Retraining Strategies

Se contemplan tres estrategias.

### Manual

Administrador solicita:

```text
POST /api/v1/models/train
```

### Scheduled

Ejemplo:

```text
Every Sunday
```

### Triggered

Reentrenamiento cuando:

```text
Model performance degrades
```

El MVP comenzará con:

```text
Manual
```

y posteriormente podrá evolucionar a:

```text
Scheduled
```

---

# 41. Continuous Learning

Smart Bakery no utilizará inicialmente online learning.

El concepto de continuous learning será:

```text
New Sales
   ↓
Dataset Update
   ↓
Periodic Retraining
   ↓
Evaluation
   ↓
Possible Promotion
```

No significa que el modelo cambie automáticamente después de cada venta.

---

# 42. Retraining Frequency

Inicialmente:

```text
Manual
```

Posteriormente podría utilizarse:

```text
Weekly
```

o:

```text
Monthly
```

dependiendo de la cantidad de datos disponibles.

La frecuencia deberá basarse en:

```text
Data volume
Model performance
Business changes
Computational cost
```

---

# 43. Training Metadata

Cada entrenamiento deberá registrar:

```text
TrainingId
ModelVersion
DatasetVersion
FeatureVersion
Algorithm
Hyperparameters
StartTime
EndTime
Duration
Metrics
Status
Error
```

Ejemplo:

```json
{
  "trainingId": "uuid",
  "modelVersion": "v4",
  "datasetVersion": "sales_2026_10",
  "featureVersion": "features_v2",
  "algorithm": "RandomForest",
  "status": "Completed",
  "durationSeconds": 84
}
```

---

# 44. Training Status

Los entrenamientos podrán tener estados:

```text
Pending
Running
Completed
Failed
Cancelled
```

Para el MVP, el entrenamiento puede ejecutarse de manera síncrona.

Posteriormente:

```text
POST /train
      ↓
202 Accepted
      ↓
Training Job
```

---

# 45. Training Failure

Si el entrenamiento falla:

```text
Training
   ↓
Failed
```

No deberá:

```text
Crear Production Model
Reemplazar Production
Eliminar Production
```

El modelo actualmente en producción debe permanecer intacto.

---

# 46. Production Safety

Una regla fundamental:

> **Un entrenamiento fallido nunca debe afectar al modelo actualmente en producción.**

Igualmente:

> **Un modelo candidato nunca debe reemplazar automáticamente al modelo de producción sin pasar por evaluación.**

---

# 47. Model Isolation

El modelo candidato deberá estar separado del modelo de producción.

Ejemplo:

```text
models/
├── production/
│   └── model_v3.joblib
│
└── candidates/
    └── model_v4.joblib
```

Alternativamente, todos pueden almacenarse juntos utilizando metadata de estado.

El sistema debe garantizar que `/predict` cargue únicamente:

```text
Production
```

---

# 48. Production Model Loading

Al iniciar Python:

```text
Python Service
      ↓
Find Production Model
      ↓
Load Artifact
      ↓
Validate
      ↓
Ready
```

Si no existe un modelo:

```text
ML service = Not Ready
```

o:

```text
ML service = Healthy
Model = Not Available
```

dependiendo de la estrategia de health checks.

---

# 49. Model Reloading

Cuando se promueva un nuevo modelo:

```text
v3 Production
       ↓
v4 Promotion
       ↓
Python reloads v4
```

El mecanismo exacto podrá ser:

```text
Restart service
```

en MVP.

Posteriormente:

```text
Hot reload
```

podrá implementarse.

---

# 50. ML Service Responsibilities

Python será responsable de:

```text
Dataset processing
Feature engineering
Training
Evaluation
Model serialization
Model loading
Prediction
ML metrics
```

Python no será responsable de:

```text
Users
Authentication
Products
Sales business rules
Promotions
Permissions
Business database transactions
```

---

# 51. .NET Responsibilities

.NET será responsable de:

```text
Authentication
Authorization
Business rules
Products
Categories
Sales
Promotions
Prediction orchestration
Model metadata
API
Persistence
Audit
```

---

# 52. Model Governance

Cada modelo debe ser trazable.

Debe poder conocerse:

```text
Quién lo entrenó o inició el entrenamiento
Cuándo se entrenó
Qué dataset utilizó
Qué features utilizó
Qué algoritmo utilizó
Qué métricas obtuvo
Quién lo promovió
Cuándo se promovió
```

En el MVP puede utilizarse:

```text
CreatedByUserId
TrainingDate
PromotedByUserId
PromotedAt
```

---

# 53. Audit Trail

Las acciones importantes deberían generar registros.

Ejemplos:

```text
Model trained
Model promoted
Model rejected
Model retired
Model rollback
```

Ejemplo:

```text
2026-10-01
Admin User
Promoted Model v4
Previous Production: v3
```

---

# 54. Dataset Audit

Los datasets también deberían ser trazables.

Ejemplo:

```text
Dataset:
sales_2026_10

Created:
2026-10-01

Records:
18,420

Date Range:
2026-01-01 → 2026-09-30

Products:
42
```

Esto facilita analizar por qué un modelo cambió de rendimiento.

---

# 55. Data Leakage Prevention

Debe evitarse utilizar información futura durante el entrenamiento.

Ejemplo incorrecto:

```text
Predict October sales
using October actual sales
```

Ejemplo correcto:

```text
Predict October sales
using historical data available before October
```

Esto es especialmente importante porque Smart Bakery utiliza series temporales.

---

# 56. Feature Availability

Las features utilizadas para una predicción deben representar información disponible en el momento de realizarla.

Ejemplo:

```text
Prediction Date:
October 15
```

No debe utilizar:

```text
Sales from October 16
```

porque produciría data leakage.

---

# 57. Experiment Tracking

En una etapa inicial se puede registrar manualmente:

```text
Algorithm
Hyperparameters
Dataset
Metrics
Training Date
```

Posteriormente podrá incorporarse:

```text
MLflow
```

para registrar:

```text
Experiments
Runs
Parameters
Metrics
Artifacts
Models
```

MLflow será opcional y no necesario para el MVP.

---

# 58. MLOps Tool Evolution

La arquitectura podrá evolucionar:

### MVP

```text
PostgreSQL
+
Python
+
joblib
+
GitHub Actions
```

### Intermediate

```text
PostgreSQL
+
Model Registry
+
Object Storage
+
Scheduled Training
+
Monitoring
```

### Advanced

```text
MLflow
+
Object Storage
+
CI/CD
+
Automated Training
+
Model Monitoring
+
Docker
+
Kubernetes
```

Docker y Kubernetes serán opcionales.

---

# 59. CI/CD Integration

El pipeline de MLOps podrá integrarse con GitHub Actions.

Ejemplo:

```text
Git Push
   ↓
Run Tests
   ↓
Validate ML Code
   ↓
Train/Test Model
   ↓
Evaluate
   ↓
Register Candidate
```

No necesariamente se deberá entrenar un modelo completo en cada push.

El pipeline de CI y el pipeline de entrenamiento deben poder funcionar independientemente.

---

# 60. CI Pipeline

La integración continua deberá comprobar:

```text
Python syntax
Python tests
Feature tests
Model tests
.NET build
.NET tests
React build
```

Ejemplo:

```text
Pull Request
     ↓
Backend Tests
     ↓
Frontend Tests
     ↓
ML Tests
     ↓
Build
     ↓
Quality Gate
```

---

# 61. Training Pipeline

El entrenamiento podrá ejecutarse separadamente:

```text
Trigger
   ↓
Create Dataset
   ↓
Train
   ↓
Evaluate
   ↓
Register Candidate
```

Triggers posibles:

```text
Manual
Schedule
API
Future automated trigger
```

---

# 62. Model Quality Gate

Antes de permitir promoción:

```text
Data Validation
        ↓
Training Successful
        ↓
Metrics Available
        ↓
Artifact Valid
        ↓
Compare Production
        ↓
Quality Gate
```

Si falla:

```text
Candidate = Rejected
```

o permanece:

```text
Candidate
```

hasta revisión administrativa, dependiendo del flujo definido.

---

# 63. ML Testing

Los tests deberán cubrir:

### Data tests

```text
Schema
Types
Nulls
Ranges
Duplicates
```

### Feature tests

```text
Feature names
Feature types
Expected calculations
```

### Model tests

```text
Model loads
Prediction returns valid output
Prediction is non-negative
```

### API tests

```text
/predict
/train
/model
/metrics
```

---

# 64. Prediction Constraints

Una predicción de demanda no debería producir valores negativos.

Si el modelo devuelve:

```text
-4.2
```

el sistema deberá manejarlo.

Una estrategia puede ser:

```text
max(0, prediction)
```

Pero la transformación debe estar documentada y aplicarse consistentemente.

---

# 65. Prediction Rounding

El modelo puede devolver:

```text
20.4
```

La UI puede mostrar:

```text
20 unidades
```

o:

```text
20.4 unidades
```

La representación y el redondeo deben definirse separadamente de la predicción interna.

Para decisiones de inventario, posteriormente podría utilizarse:

```text
ceil(prediction)
```

si el negocio lo requiere.

---

# 66. Monitoring Dashboard

El dashboard de MLOps podrá mostrar:

```text
Current Production Model
Model Version
Algorithm
MAE
RMSE
R²
Training Date
Dataset Version
Prediction Count
Recent Prediction Error
```

Ejemplo:

```text
┌───────────────────────────────────┐
│ Production Model                  │
│ v3 — RandomForest                 │
│                                   │
│ MAE     3.91                      │
│ RMSE    5.42                      │
│ R²      0.83                      │
│                                   │
│ Dataset: sales_2026_09            │
└───────────────────────────────────┘
```

---

# 67. Alerts

Posteriormente podrán implementarse alertas cuando:

```text
Model error increases significantly
Data validation fails
Training fails
ML service unavailable
No production model exists
Prediction error exceeds threshold
```

Las alertas pueden comenzar simplemente como logs.

Posteriormente:

```text
Email
Webhook
Monitoring platform
```

---

# 68. MLOps Security

Los procesos de entrenamiento deben respetar la seguridad existente.

```text
Admin
   ↓
.NET Authorization
   ↓
Training Request
   ↓
Python
```

El frontend nunca debe poder:

```text
Directly train Python
Directly upload models
Change production model
Modify model metadata
```

---

# 69. Model Artifact Security

Los artifacts deberán tratarse como archivos controlados.

No se debe permitir que un usuario común pueda:

```text
Upload arbitrary .joblib
Replace production artifact
Modify model files
```

Los artifacts deben ser generados por el pipeline de entrenamiento o por procesos administrativos controlados.

---

# 70. Docker Compatibility

MLOps debe funcionar sin Docker.

Desarrollo:

```text
Python
.NET
PostgreSQL
React
```

Docker podrá añadirse posteriormente:

```text
Docker
├── frontend
├── backend
├── ml-service
└── postgres
```

La lógica de Machine Learning no debe depender de Docker.

---

# 71. Kubernetes Compatibility

Kubernetes tampoco será obligatorio.

Posteriormente podría desplegarse:

```text
React
    ↓
.NET Deployment
    ↓
Python ML Deployment
    ↓
PostgreSQL
```

y:

```text
Model Storage
```

mediante un servicio externo.

La arquitectura debe permitir esta evolución sin modificar las reglas principales del dominio.

---

# 72. Local Development

El flujo local será:

```text
PostgreSQL
     ↑
     │
.NET API
     ↑
     │
React

.NET API
     │
     ↓
Python ML API
```

Cada componente puede ejecutarse independientemente.

---

# 73. MVP MLOps Scope

El MVP deberá incluir:

```text
[ ] Dataset generation
[ ] Dataset validation
[ ] Feature engineering
[ ] Baseline model
[ ] ML model training
[ ] Model evaluation
[ ] MAE
[ ] RMSE
[ ] R²
[ ] Model versioning
[ ] Dataset versioning
[ ] Model metadata
[ ] Local model artifacts
[ ] Candidate status
[ ] Production status
[ ] Rejected status
[ ] Retired status
[ ] Manual promotion
[ ] Prediction persistence
[ ] Production model loading
[ ] Basic prediction monitoring
```

---

# 74. Post-MVP MLOps Scope

Después del MVP:

```text
[ ] Automated retraining
[ ] Scheduled training
[ ] Automatic quality gates
[ ] Prediction vs actual monitoring
[ ] Data drift detection
[ ] Model drift detection
[ ] Experiment tracking
[ ] MLflow
[ ] Object storage
[ ] Automated rollback
[ ] Alerting
[ ] Advanced observability
```

---

# 75. Advanced MLOps Scope

Como etapa avanzada:

```text
[ ] Docker
[ ] Container Registry
[ ] Kubernetes
[ ] Model deployment automation
[ ] Horizontal scaling
[ ] Distributed training
[ ] Advanced monitoring
[ ] Automated model promotion
[ ] Feature Store
```

Estos componentes no forman parte del MVP.

---

# 76. Complete MLOps Flow

El flujo final esperado será:

```text
                         ┌──────────────────┐
                         │   PostgreSQL     │
                         │ Historical Sales │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │ Dataset Pipeline │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │ Feature Pipeline │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │ Training Pipeline│
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │   Evaluation     │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │  Model Registry  │
                         └────────┬─────────┘
                                  │
                         ┌────────┴─────────┐
                         │                  │
                         ▼                  ▼
                    Candidate          Production
                         │                  │
                         │                  ▼
                         │             Prediction
                         │                  │
                         │                  ▼
                         │             Monitoring
                         │                  │
                         └──────────┬───────┘
                                    │
                                    ▼
                              Retraining
```

---

# 77. Core MLOps Principles

Smart Bakery seguirá estos principios:

```text
1. Every model must be versioned.

2. Every model must be traceable to its dataset.

3. Features used during prediction must be consistent
   with features used during training.

4. Production models must be protected.

5. Candidate models must be evaluated before promotion.

6. Failed training must never replace production.

7. Previous production models must remain available
   for rollback and auditing.

8. Predictions must identify which model generated them.

9. Data quality must be validated before training.

10. Time-series data must respect chronological order.

11. Data leakage must be prevented.

12. Retraining does not automatically mean deployment.

13. Continuous learning initially means periodic retraining,
    not online learning.

14. MLOps must work without Docker or Kubernetes.

15. Infrastructure can evolve without changing business logic.

16. Monitoring should eventually measure real-world
    prediction performance.

17. Automation should be introduced progressively.
```

---

# 78. Definition of Done

MLOps estará correctamente implementado para el MVP cuando:

```text
[ ] Historical sales can generate a dataset.
[ ] Dataset validation works.
[ ] Features can be generated consistently.
[ ] Baseline exists.
[ ] At least one ML model can be trained.
[ ] Model metrics are calculated.
[ ] Model artifact can be saved.
[ ] Model version is registered.
[ ] Dataset version is registered.
[ ] Candidate models are isolated from production.
[ ] Production model can be identified.
[ ] Predictions use only the production model.
[ ] Predictions store the model version.
[ ] Candidate models can be promoted.
[ ] Candidate models can be rejected.
[ ] Production models can become retired.
[ ] Previous models remain available.
[ ] Training failures do not affect production.
[ ] Basic prediction monitoring exists.
[ ] Retraining can be performed manually.
[ ] CI can validate ML code.
[ ] The entire MLOps workflow works locally
    without Docker or Kubernetes.
```

---

# 79. Final MLOps Architecture

```text
                           SMART BAKERY
                                │
                ┌───────────────┴───────────────┐
                │                               │
          Business System                 ML System
                │                               │
                ▼                               ▼
          .NET API                         Python API
                │                               │
        ┌───────┴────────┐             ┌────────┴─────────┐
        │                │             │                  │
        ▼                ▼             ▼                  ▼
   PostgreSQL       ML Client      Training           Prediction
        │                │             │                  │
        │                └─────────────┴──────────────────┘
        │                              │
        ▼                              ▼
 Historical Data                  Model Registry
                                       │
                              ┌────────┴────────┐
                              │                 │
                              ▼                 ▼
                         Candidate         Production
                              │                 │
                              │                 ▼
                              │            Predictions
                              │                 │
                              └────────┬────────┘
                                       │
                                       ▼
                                   Monitoring
                                       │
                                       ▼
                                   Retraining
```

---

# 80. Core Principle

> **MLOps en Smart Bakery no consiste solamente en entrenar un modelo. Consiste en construir un ciclo controlado y reproducible donde los datos generan modelos versionados, los modelos son evaluados antes de producción, las predicciones pueden rastrearse hasta el modelo que las generó y el sistema puede mejorar mediante nuevos entrenamientos sin poner en riesgo el modelo actualmente utilizado.**


# 81. Concrete API Contracts

Esta sección define los contratos HTTP concretos entre el backend `.NET` y el servicio de Machine Learning en `Python`.

El principio es:

```text
React
  ↓
.NET API
  ↓
Python ML API
  ↓
Model
```

El frontend **nunca** debe consumir directamente el Python ML API.

---

## 81.1 ML Service Base URL

En desarrollo:

```text
http://localhost:8000
```

En `.NET`:

```json
{
  "MachineLearning": {
    "BaseUrl": "http://localhost:8000"
  }
}
```

La URL debe configurarse mediante configuración/environment variables.

No debe estar hardcoded en código de negocio.

---

# 82. Health Contract

## Request

```http
GET /health
```

No requiere authentication cuando el servicio está ejecutándose dentro de una red privada/local y el endpoint no expone información sensible.

## Response

```http
200 OK
Content-Type: application/json
```

```json
{
  "status": "healthy",
  "service": "smart-bakery-ml",
  "modelLoaded": true
}
```

Possible response when the service is running but no production model is available:

```json
{
  "status": "degraded",
  "service": "smart-bakery-ml",
  "modelLoaded": false
}
```

---

# 83. Get Production Model

## Request

```http
GET /model
```

Este endpoint debe devolver información sobre el modelo actualmente cargado.

## Response

```http
200 OK
Content-Type: application/json
```

```json
{
  "version": "v3",
  "algorithm": "RandomForest",
  "datasetVersion": "sales_2026_09",
  "featureVersion": "features_v2",
  "trainedAt": "2026-09-30T18:30:00Z",
  "metrics": {
    "mae": 3.91,
    "rmse": 5.42,
    "r2": 0.83
  }
}
```

Si no existe un modelo:

```http
404 Not Found
```

```json
{
  "code": "production_model_not_found",
  "message": "No production model is currently available."
}
```

---

# 84. Prediction Contract

## Request

```http
POST /predict
Content-Type: application/json
```

Request body:

```json
{
  "productId": "3f7c1d5e-5a7b-4c2f-9b31-123456789abc",
  "predictionDate": "2026-10-15",
  "features": {
    "dayOfWeek": 4,
    "dayOfMonth": 15,
    "month": 10,
    "weekOfYear": 42,
    "isWeekend": false,
    "season": "autumn",
    "price": 4.5,
    "promotion": false,
    "previousDaySales": 18,
    "previous7DaysAverage": 21.4,
    "previous30DaysAverage": 19.8
  }
}
```

## Response

```http
200 OK
Content-Type: application/json
```

```json
{
  "productId": "3f7c1d5e-5a7b-4c2f-9b31-123456789abc",
  "predictionDate": "2026-10-15",
  "predictedQuantity": 22.37,
  "model": {
    "version": "v3",
    "algorithm": "RandomForest"
  }
}
```

El Python service debe utilizar exclusivamente el modelo `Production`.

El cliente no debe poder indicar:

```json
{
  "modelVersion": "v1"
}
```

para seleccionar arbitrariamente un modelo.

La selección del modelo de producción es responsabilidad del sistema.

---

# 85. Prediction Validation

El ML API debe rechazar requests inválidos.

Ejemplo:

```http
422 Unprocessable Entity
```

```json
{
  "code": "invalid_prediction_request",
  "message": "Prediction date must be provided."
}
```

También debe rechazarse:

```text
price < 0
previousDaySales < 0
previous7DaysAverage < 0
previous30DaysAverage < 0
missing required feature
invalid product identifier
invalid prediction date
```

---

# 86. Training Contract

## Request

```http
POST /train
Content-Type: application/json
```

Request:

```json
{
  "datasetVersion": "sales_2026_10",
  "featureVersion": "features_v2",
  "algorithm": "RandomForest",
  "parameters": {
    "nEstimators": 200,
    "maxDepth": 12,
    "randomState": 42
  }
}
```

---

# 87. Training Response

For the MVP, training may be synchronous.

Successful response:

```http
200 OK
Content-Type: application/json
```

```json
{
  "trainingId": "6a2a9b8e-4a25-4d6a-b8f2-123456789abc",
  "status": "completed",
  "model": {
    "version": "v4",
    "algorithm": "RandomForest",
    "datasetVersion": "sales_2026_10",
    "featureVersion": "features_v2"
  },
  "metrics": {
    "mae": 3.72,
    "rmse": 5.11,
    "r2": 0.85
  },
  "artifact": {
    "path": "models/model_v4.joblib"
  }
}
```

El modelo creado por `/train` debe ser registrado como:

```text
Candidate
```

No debe convertirse automáticamente en:

```text
Production
```

---

# 88. Training Failure

Si el entrenamiento falla:

```http
500 Internal Server Error
```

```json
{
  "trainingId": "6a2a9b8e-4a25-4d6a-b8f2-123456789abc",
  "status": "failed",
  "error": {
    "code": "training_failed",
    "message": "Training pipeline failed during model fitting."
  }
}
```

El modelo actualmente en producción no debe modificarse.

---

# 89. Async Training — Future Contract

Cuando el proyecto evolucione a entrenamiento asíncrono:

```http
POST /train
```

deberá devolver:

```http
202 Accepted
```

```json
{
  "trainingId": "6a2a9b8e-4a25-4d6a-b8f2-123456789abc",
  "status": "pending"
}
```

Posteriormente podría existir:

```http
GET /train/{trainingId}
```

Response:

```json
{
  "trainingId": "6a2a9b8e-4a25-4d6a-b8f2-123456789abc",
  "status": "running",
  "progress": 65
}
```

Estados:

```text
pending
running
completed
failed
cancelled
```

Esta funcionalidad queda fuera del MVP.

---

# 90. Metrics Contract

## Request

```http
GET /metrics
```

## Response

```http
200 OK
Content-Type: application/json
```

```json
{
  "model": {
    "version": "v3",
    "algorithm": "RandomForest"
  },
  "metrics": {
    "mae": 3.91,
    "rmse": 5.42,
    "r2": 0.83
  },
  "evaluation": {
    "datasetVersion": "sales_2026_09",
    "evaluatedAt": "2026-09-30T18:30:00Z"
  }
}
```

---

# 91. .NET → Python Client Contract

El backend debe encapsular la comunicación mediante una abstracción:

```csharp
public interface IMachineLearningClient
{
    Task<PredictionResponse> PredictAsync(
        PredictionRequest request,
        CancellationToken cancellationToken);

    Task<TrainingResponse> TrainAsync(
        TrainingRequest request,
        CancellationToken cancellationToken);

    Task<ModelResponse> GetProductionModelAsync(
        CancellationToken cancellationToken);

    Task<HealthResponse> GetHealthAsync(
        CancellationToken cancellationToken);
}
```

El Application layer no debe conocer detalles de `HttpClient`.

La implementación concreta debe pertenecer a Infrastructure:

```text
SmartBakery.Infrastructure
└── MachineLearning
    ├── MachineLearningClient.cs
    ├── MachineLearningOptions.cs
    └── Contracts/
```

---

# 92. .NET Prediction Contract

El frontend utilizará el .NET API:

```http
POST /api/v1/predictions
```

No:

```http
POST http://localhost:8000/predict
```

El flujo será:

```text
React
  │
  │ POST /api/v1/predictions
  ▼
.NET API
  │
  │ POST /predict
  ▼
Python
  │
  ▼
Model
```

El .NET API transforma el request del frontend al contrato interno del ML service.

---

# 93. .NET Prediction Request

Frontend:

```json
{
  "productId": "3f7c1d5e-5a7b-4c2f-9b31-123456789abc",
  "predictionDate": "2026-10-15"
}
```

El cliente React no necesita enviar directamente todas las features.

El backend debe:

```text
Product
    ↓
Sales History
    ↓
Promotion
    ↓
Historical Features
    ↓
ML Request
```

Por ejemplo:

```json
{
  "productId": "3f7c1d5e-5a7b-4c2f-9b31-123456789abc",
  "predictionDate": "2026-10-15",
  "features": {
    "dayOfWeek": 4,
    "month": 10,
    "isWeekend": false,
    "price": 4.5,
    "promotion": false,
    "previousDaySales": 18,
    "previous7DaysAverage": 21.4,
    "previous30DaysAverage": 19.8
  }
}
```

Esto mantiene las reglas de negocio y composición de datos en `.NET`.

---

# 94. .NET Training Contract

Frontend/Admin:

```http
POST /api/v1/models/train
```

Request:

```json
{
  "algorithm": "RandomForest"
}
```

El backend valida:

```text
Authentication
Authorization
Algorithm
Training permissions
```

Después solicita al Python service:

```http
POST /train
```

El backend registra el resultado en PostgreSQL.

---

# 95. Model Promotion Contract

La promoción no se realiza desde Python.

Se realiza mediante:

```http
POST /api/v1/models/{id}/promote
```

Authorization:

```text
Admin only
```

El flujo será:

```text
Admin
  ↓
.NET API
  ↓
Validate Candidate
  ↓
Validate Metrics
  ↓
Promote Model
  ↓
Update PostgreSQL
  ↓
Notify/Reload ML Service
```

Python no decide qué modelo es oficialmente `Production`.

---

# 96. Model Rejection Contract

```http
POST /api/v1/models/{id}/reject
```

Request:

```json
{
  "reason": "MAE is higher than the current production model."
}
```

Response:

```http
200 OK
```

```json
{
  "id": "model-id",
  "version": "v4",
  "status": "Rejected",
  "rejectedReason": "MAE is higher than the current production model."
}
```

---

# 97. Model Metadata Contract

```http
GET /api/v1/models/{id}
```

Response:

```json
{
  "id": "model-id",
  "version": "v4",
  "algorithm": "RandomForest",
  "status": "Candidate",
  "datasetVersion": "sales_2026_10",
  "featureVersion": "features_v2",
  "metrics": {
    "mae": 3.72,
    "rmse": 5.11,
    "r2": 0.85
  },
  "artifactPath": "models/model_v4.joblib",
  "trainingDate": "2026-10-01T10:30:00Z"
}
```

---

# 98. API Error Contract

.NET APIs utilizarán `ProblemDetails` / RFC 7807.

Ejemplo:

```http
400 Bad Request
Content-Type: application/problem+json
```

```json
{
  "type": "https://api.smart-bakery.local/errors/validation",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/models/train",
  "errors": {
    "algorithm": [
      "Algorithm is required."
    ]
  }
}
```

Para errores del Python service:

```http
502 Bad Gateway
```

cuando `.NET` no puede completar correctamente la comunicación con el ML service.

Ejemplo:

```json
{
  "type": "https://api.smart-bakery.local/errors/ml-service",
  "title": "Machine Learning service unavailable",
  "status": 502,
  "detail": "The prediction could not be completed because the ML service is unavailable."
}
```

Los detalles internos de Python no deben exponerse al frontend.

---

# 99. Timeout and Resilience Contract

El `IMachineLearningClient` deberá utilizar:

```text
Timeout
CancellationToken
Logging
Retry where appropriate
Circuit breaker where appropriate
```

No se deben realizar retries indiscriminados sobre operaciones que puedan provocar efectos secundarios.

Por ejemplo:

```text
GET /model
```

puede ser retriable.

Mientras que:

```text
POST /train
```

requiere mayor cuidado para evitar iniciar múltiples entrenamientos accidentalmente.

---

# 100. Contract Ownership

La responsabilidad de cada contrato será:

| Contract              | Owner  |
| --------------------- | ------ |
| `/api/v1/predictions` | .NET   |
| `/api/v1/models`      | .NET   |
| `/predict`            | Python |
| `/train`              | Python |
| `/model`              | Python |
| `/metrics`            | Python |
| `/health`             | Python |

El `.NET API` es el contrato público del sistema.

El Python API es un contrato interno.

---

# 101. Contract Evolution

Todos los contratos públicos del sistema utilizarán:

```text
/api/v1
```

Ejemplo:

```text
/api/v1/products
/api/v1/sales
/api/v1/predictions
/api/v1/models
```

Los contratos internos de Python inicialmente no necesitan versionado en URL.

Si posteriormente existen breaking changes importantes:

```text
/predict
```

podrá evolucionar a:

```text
/api/v2/predict
```

o mediante versionado de schema.

---

# 102. API Contract Principles

Los contratos deben respetar:

```text
1. Explicit request schemas.

2. Explicit response schemas.

3. Validation at the boundary.

4. Stable public API.

5. Internal ML API remains private.

6. React never talks directly to Python.

7. React never talks directly to PostgreSQL.

8. .NET owns business authorization.

9. Python owns ML operations.

10. Model version is always traceable.

11. Production model cannot be selected arbitrarily
    by the client.

12. Training failures cannot replace production.

13. Errors must not expose internal implementation details.

14. API contracts must be testable independently.
```

---

# 103. Contract Testing

Los contratos deberán cubrirse mediante tests.

### .NET API

```text
POST /api/v1/predictions
POST /api/v1/models/train
POST /api/v1/models/{id}/promote
POST /api/v1/models/{id}/reject
```

### Python API

```text
GET /health
GET /model
GET /metrics
POST /predict
POST /train
```

Los tests deberán verificar:

```text
Request schema
Response schema
HTTP status
Validation
Authentication
Authorization
Error contract
```

---

# 104. End-to-End MLOps Contract

El flujo completo esperado será:

```text
1. Admin requests training

POST /api/v1/models/train
        ↓
.NET
        ↓
POST /train
        ↓
Python
        ↓
Training
        ↓
Evaluation
        ↓
Candidate
        ↓
.NET stores metadata


2. Admin reviews candidate

GET /api/v1/models/{id}
        ↓
Metrics
        ↓
Candidate


3. Admin promotes model

POST /api/v1/models/{id}/promote
        ↓
Production


4. Employee requests prediction

POST /api/v1/predictions
        ↓
.NET
        ↓
POST /predict
        ↓
Python
        ↓
Production Model
        ↓
Prediction
        ↓
.NET
        ↓
PostgreSQL
        ↓
React
```

Este contrato representa el **MVP completo de integración MLOps** entre React, .NET, PostgreSQL y Python.
