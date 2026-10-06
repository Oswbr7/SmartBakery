# 🍰 Smart Bakery

> Plataforma inteligente para la gestión y predicción de demanda de una pastelería.

---

## 1. Descripción del proyecto

**Smart Bakery** es una plataforma web para la gestión de una pastelería que incorpora un sistema de **Machine Learning capaz de aprender de las ventas históricas y predecir la demanda futura de productos**.

El objetivo principal del proyecto no es únicamente construir un sistema administrativo, sino utilizarlo como proyecto de aprendizaje para desarrollar conocimientos prácticos en:

* Machine Learning
* Data Science
* MLOps
* CI/CD
* Arquitectura de software
* APIs
* React
* .NET
* Python
* PostgreSQL
* Testing
* Automatización
* Opcionalmente Docker y Kubernetes

La aplicación permitirá registrar productos y ventas, analizar el comportamiento histórico y generar predicciones sobre la cantidad de productos que probablemente se venderán en determinadas fechas.

---

# 2. Objetivos

## 2.1 Objetivo principal

Construir una plataforma capaz de utilizar datos históricos de ventas para entrenar un modelo de Machine Learning que permita **predecir la demanda futura de productos de una pastelería**.

## 2.2 Objetivos secundarios

El proyecto debe permitir:

* Administrar productos.
* Registrar ventas.
* Consultar historial de ventas.
* Visualizar métricas.
* Generar datos históricos.
* Entrenar modelos de Machine Learning.
* Evaluar modelos.
* Comparar versiones de modelos.
* Generar predicciones.
* Registrar nuevos datos.
* Reentrenar modelos periódicamente.
* Mantener el modelo anterior si un nuevo modelo tiene peor rendimiento.
* Automatizar pruebas mediante CI/CD.
* Preparar el proyecto para Docker y Kubernetes sin depender de ellos.

---

# 3. Alcance inicial

La primera versión debe enfocarse en:

1. Gestión de productos.
2. Registro de ventas.
3. Persistencia de datos.
4. Dashboard.
5. Dataset histórico.
6. Primer modelo de Machine Learning.
7. API de predicciones.
8. Integración entre .NET y Python.
9. Tests.
10. Pipeline básico de CI/CD.

Docker y Kubernetes quedan fuera del requisito inicial.

---

# 4. Arquitectura general

```text
                         ┌──────────────────────┐
                         │       React          │
                         │      Frontend        │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │      .NET 10         │
                         │       API            │
                         │                      │
                         │ Products             │
                         │ Sales                │
                         │ Dashboard            │
                         │ Predictions          │
                         └───────┬───────┬──────┘
                                 │       │
                    ┌────────────┘       └─────────────┐
                    ▼                                  ▼
          ┌──────────────────┐              ┌──────────────────┐
          │   PostgreSQL     │              │   Python API     │
          │                  │              │     FastAPI      │
          │ Products         │              │                  │
          │ Sales            │              │ Prediction       │
          │ Models           │              │ Training         │
          │ Predictions      │              │ Evaluation       │
          └──────────────────┘              └────────┬─────────┘
                                                     │
                                                     ▼
                                           ┌──────────────────┐
                                           │ Machine Learning │
                                           │                  │
                                           │ Pandas           │
                                           │ Scikit-learn     │
                                           │ ML Models        │
                                           └──────────────────┘
```

---

# 5. Tecnologías

## Frontend

* React
* TypeScript
* Vite
* TanStack Query
* React Hook Form
* Zustand
* shadcn/ui
* Recharts

## Backend

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* FluentValidation
* MediatR
* CQRS
* JWT Authentication

## Machine Learning

* Python
* FastAPI
* Pandas
* NumPy
* Scikit-learn
* Joblib

Opcional posteriormente:

* MLflow
* XGBoost
* LightGBM

## DevOps

* Git
* GitHub
* GitHub Actions

Opcional:

* Docker
* Docker Compose
* Kubernetes

---

# 6. Estructura del proyecto

Se recomienda utilizar un repositorio principal:

```text
smart-bakery/
│
├── frontend/
│   └── smart-bakery-web/
│
├── backend/
│   └── SmartBakery/
│
├── ml-service/
│   ├── app/
│   ├── models/
│   ├── training/
│   ├── datasets/
│   ├── tests/
│   └── requirements.txt
│
├── docs/
│   ├── architecture/
│   ├── ml/
│   └── api/
│
├── scripts/
│
├── .github/
│   └── workflows/
│
├── README.md
└── .gitignore
```

---

# 7. Módulos principales

## 7.1 Productos

La aplicación debe permitir:

* Crear productos.
* Editar productos.
* Activar/desactivar productos.
* Consultar productos.
* Consultar precio.
* Clasificar productos.

Ejemplos:

```text
Chocolate
Tres Leches
Red Velvet
Vainilla
Fresa
Zanahoria
Cheesecake
```

### Datos mínimos

```text
Product
---------
Id
Name
Description
Category
Price
IsActive
CreatedAt
UpdatedAt
```

---

# 8. Ventas

Una venta representa una transacción realizada por uno o varios productos.

### Sale

```text
Sale
---------
Id
SaleDate
TotalAmount
PromotionApplied
CreatedAt
```

### SaleItem

```text
SaleItem
---------
Id
SaleId
ProductId
Quantity
UnitPrice
Subtotal
```

La cantidad vendida será uno de los datos fundamentales utilizados por el modelo de Machine Learning.

---

# 9. Variables para Machine Learning

El modelo debe utilizar inicialmente variables relacionadas con:

### Temporales

* Día de la semana.
* Día del mes.
* Mes.
* Semana del año.
* Año.
* Fin de semana.
* Temporada.

### Producto

* Producto.
* Categoría.
* Precio.

### Ventas históricas

* Ventas del día anterior.
* Ventas de los últimos 7 días.
* Promedio de ventas de los últimos 7 días.
* Promedio de ventas de los últimos 30 días.

### Promociones

* Si existió promoción.
* Tipo de promoción.
* Porcentaje de descuento.

### Eventos

Opcional inicialmente:

* Festivo.
* Día especial.
* Evento local.

---

# 10. Dataset

El proyecto debe utilizar un dataset histórico.

Inicialmente se puede generar un dataset sintético para desarrollar el modelo.

Ejemplo:

```csv
date,product,quantity,price,promotion,day_of_week,is_holiday
2025-01-01,Chocolate,12,350,false,3,true
2025-01-02,Chocolate,8,350,false,4,false
2025-01-03,Chocolate,15,350,true,5,false
```

El dataset sintético debe intentar representar comportamientos realistas:

* Mayor demanda durante fines de semana.
* Mayor demanda durante determinadas temporadas.
* Variaciones aleatorias.
* Incrementos derivados de promociones.
* Diferencias entre productos.

El objetivo no es crear datos perfectamente predecibles.

---

# 11. Machine Learning

## 11.1 Primer objetivo

El primer modelo debe predecir:

> Cantidad esperada de unidades vendidas de un producto en una fecha determinada.

Ejemplo:

```text
Producto:
Chocolate

Fecha:
2026-10-10

Predicción:
17 unidades
```

---

# 12. Modelos iniciales

No se debe comenzar directamente con Deep Learning.

Primero se deben experimentar modelos clásicos.

Orden sugerido:

```text
1. Baseline
2. Linear Regression
3. Random Forest
4. Gradient Boosting
5. XGBoost (opcional)
```

El objetivo es comprender:

* Features.
* Training.
* Validation.
* Overfitting.
* Underfitting.
* Métricas.
* Feature importance.
* Predicciones.

---

# 13. Baseline

Antes de entrenar un modelo complejo debe existir un baseline.

Ejemplo:

```text
Predicción = promedio de ventas de los últimos 7 días
```

El modelo de Machine Learning deberá superar este baseline para considerarse útil.

---

# 14. Métricas

Las primeras métricas serán:

* MAE
* RMSE
* R²

### MAE

Representa el error absoluto promedio.

Ejemplo:

```text
MAE = 3.2
```

Significa que, en promedio, la predicción se desvía aproximadamente 3.2 unidades.

La métrica principal inicialmente será **MAE**, debido a que resulta fácil de interpretar para un problema de predicción de unidades.

---

# 15. Training Pipeline

El servicio de Machine Learning debe contar con un proceso de entrenamiento.

```text
Dataset
   │
   ▼
Data Cleaning
   │
   ▼
Feature Engineering
   │
   ▼
Train / Validation Split
   │
   ▼
Model Training
   │
   ▼
Evaluation
   │
   ▼
Metrics
   │
   ▼
Model Artifact
```

El modelo entrenado debe almacenarse como un artefacto versionado.

Ejemplo:

```text
models/
├── model_v1.joblib
├── model_v2.joblib
└── model_v3.joblib
```

---

# 16. Versionado de modelos

Cada entrenamiento debe generar una versión.

Ejemplo:

```text
Model v1
MAE: 5.42

Model v2
MAE: 4.91

Model v3
MAE: 3.87
```

Debe almacenarse información como:

```text
ModelVersion
--------------
Id
Version
Algorithm
MAE
RMSE
R2
TrainingDate
DatasetVersion
IsProduction
```

---

# 17. Selección automática del modelo

Cuando se entrene un nuevo modelo:

```text
Nuevo modelo
      │
      ▼
Evaluar
      │
      ▼
Comparar con producción
      │
      ├───────────────┐
      ▼               ▼
   Peor/igual        Mejor
      │               │
      ▼               ▼
  No activar      Activar como
                  producción
```

No se debe reemplazar automáticamente un modelo de producción si el nuevo modelo tiene peor rendimiento según las métricas definidas.

---

# 18. API de Machine Learning

El servicio Python utilizará FastAPI.

Endpoints iniciales:

```http
GET /health
```

Comprueba que el servicio funciona.

---

```http
POST /predict
```

Realiza una predicción.

Request:

```json
{
  "productId": 1,
  "date": "2026-10-10",
  "promotion": true
}
```

Response:

```json
{
  "predictedQuantity": 17,
  "modelVersion": "v3"
}
```

---

```http
POST /train
```

Inicia entrenamiento.

---

```http
GET /model
```

Devuelve información del modelo actualmente utilizado.

---

```http
GET /metrics
```

Devuelve las métricas del modelo.

---

# 19. Integración .NET → Python

El frontend no debe comunicarse directamente con el servicio de Machine Learning.

La comunicación será:

```text
React
  │
  ▼
.NET API
  │
  ▼
Python ML API
  │
  ▼
Model
```

Esto permitirá mantener separadas las responsabilidades.

---

# 20. Dashboard

El dashboard debe mostrar información relevante para el negocio.

## KPIs

```text
Ventas del día
Ventas del mes
Productos vendidos
Producto más vendido
Predicción próxima semana
```

## Gráficas

### Ventas

```text
Ventas últimos 30 días
```

### Productos

```text
Distribución de ventas por producto
```

### Predicciones

```text
Ventas reales
vs
Ventas predichas
```

### Rendimiento del modelo

```text
MAE
RMSE
R²
Versión actual
```

---

# 21. Vista de predicciones

Debe existir una sección específica para consultar predicciones.

Ejemplo:

```text
Predicciones

Fecha: 10/10/2026

Producto          Predicción

Chocolate             17
Fresa                  9
Vainilla              11
Tres Leches           14
Red Velvet             8
```

También debe poder visualizarse:

```text
Ventas reales
        vs
Predicciones
```

cuando existan datos reales para la fecha.

---

# 22. Reentrenamiento

El sistema debe estar preparado para reentrenar periódicamente el modelo.

Inicialmente puede ejecutarse manualmente:

```text
POST /train
```

Posteriormente se puede automatizar.

Ejemplo conceptual:

```text
Cada domingo
     ↓
Extraer datos
     ↓
Crear dataset
     ↓
Entrenar
     ↓
Evaluar
     ↓
Comparar
     ↓
Registrar
     ↓
Activar si mejora
```

---

# 23. Aprendizaje continuo

El proyecto debe simular un escenario de aprendizaje continuo.

```text
            Nuevas ventas
                 │
                 ▼
             Base de datos
                 │
                 ▼
           Dataset actualizado
                 │
                 ▼
            Nuevo entrenamiento
                 │
                 ▼
              Evaluación
                 │
          ┌──────┴──────┐
          ▼             ▼
       No mejora      Mejora
          │             │
          ▼             ▼
     Mantener       Nuevo modelo
     producción     producción
```

La primera implementación no necesita utilizar aprendizaje online.

El aprendizaje continuo será mediante **reentrenamiento periódico con datos nuevos**.

---

# 24. CI/CD

El proyecto debe utilizar GitHub Actions.

## Pipeline de Backend

```text
Push / Pull Request
        ↓
Checkout
        ↓
Restore
        ↓
Build
        ↓
Unit Tests
        ↓
Integration Tests
```

## Pipeline Frontend

```text
Push / Pull Request
        ↓
npm install
        ↓
Lint
        ↓
Tests
        ↓
Build
```

## Pipeline ML

```text
Push / Pull Request
        ↓
Install dependencies
        ↓
Lint
        ↓
pytest
        ↓
Train model
        ↓
Evaluate
        ↓
Validate metrics
```

---

# 25. CI/CD de producción

Posteriormente:

```text
GitHub
   │
   ▼
Pull Request
   │
   ▼
Tests
   │
   ▼
Merge
   │
   ▼
Main
   │
   ▼
Build
   │
   ▼
Deploy
```

El método concreto de deployment puede definirse posteriormente.

---

# 26. Docker

Docker es **opcional**.

El proyecto NO debe depender de Docker para desarrollo local.

La aplicación debe poder ejecutarse directamente:

```text
React
npm run dev

.NET
dotnet run

Python
uvicorn app.main:app
```

Posteriormente se podrán crear:

```text
Dockerfile
Docker Compose
```

para:

* Frontend.
* Backend.
* ML API.
* PostgreSQL.

---

# 27. Kubernetes

Kubernetes también es **opcional**.

No debe formar parte de los requisitos para completar el proyecto.

Se podrá agregar posteriormente como una fase de aprendizaje de infraestructura.

Posible arquitectura futura:

```text
                 Kubernetes
                     │
       ┌─────────────┼─────────────┐
       │             │             │
       ▼             ▼             ▼
    React          .NET         Python ML
                                  API
       │             │             │
       └─────────────┼─────────────┘
                     │
                     ▼
                 PostgreSQL
```

Posibles conceptos a estudiar:

* Pods
* Deployments
* Services
* ConfigMaps
* Secrets
* Ingress
* Health Checks
* Horizontal Pod Autoscaler
* Rolling Updates

---

# 28. Testing

## Backend

Utilizar:

* xUnit
* Moq/NSubstitute
* Integration Tests

Tests importantes:

```text
Crear producto
Editar producto
Registrar venta
Calcular total
Consultar ventas
Solicitar predicción
```

## Frontend

Tests para:

* Componentes.
* Formularios.
* Dashboard.
* Manejo de errores.
* Consultas API.

## Machine Learning

Utilizar:

* pytest

Tests importantes:

```text
Dataset válido
Feature engineering
Training pipeline
Predicción
Métricas
Carga del modelo
```

---

# 29. Seguridad

La API deberá utilizar autenticación.

Inicialmente:

```text
JWT
```

Roles:

```text
Admin
Employee
```

Permisos básicos:

```text
Admin
- Gestionar productos
- Consultar ventas
- Entrenar modelos
- Consultar métricas

Employee
- Registrar ventas
- Consultar productos
- Consultar predicciones
```

El endpoint de entrenamiento no debe estar disponible para cualquier usuario.

---

# 30. Manejo de errores

El backend deberá tener un manejo global de excepciones.

Las respuestas deben utilizar un formato consistente.

Ejemplo:

```json
{
  "status": 400,
  "message": "The product does not exist.",
  "errors": []
}
```

El servicio Python también debe manejar:

* Modelo inexistente.
* Dataset inválido.
* Datos incompletos.
* Error de predicción.
* Error de entrenamiento.

---

# 31. Observabilidad

Posteriormente se pueden incorporar:

* Logs estructurados.
* Health checks.
* Métricas de API.
* Tiempo de respuesta.
* Errores de predicción.
* Historial de entrenamientos.

No es necesario implementar observabilidad avanzada en la primera versión.

---

# 32. Fases de desarrollo

## Fase 1 — Setup

* Crear repositorio.
* Crear estructura.
* Configurar React.
* Configurar .NET.
* Configurar Python.
* Configurar PostgreSQL.
* Configurar Git.

---

## Fase 2 — Backend

* Arquitectura.
* Productos.
* Ventas.
* Base de datos.
* Migraciones.
* Validaciones.
* Tests.

---

## Fase 3 — Frontend

* Layout.
* Login.
* Productos.
* Ventas.
* Dashboard.
* Gráficas.

---

## Fase 4 — Dataset

* Crear generador de datos.
* Generar historial.
* Limpiar datos.
* Analizar distribución.
* Crear dataset para ML.

---

## Fase 5 — Machine Learning

* Crear baseline.
* Feature engineering.
* Entrenar primer modelo.
* Evaluar.
* Comparar modelos.
* Guardar modelo.
* Crear versión.

---

## Fase 6 — ML API

* FastAPI.
* `/predict`.
* `/train`.
* `/model`.
* `/metrics`.
* Integración con .NET.

---

## Fase 7 — Integración

```text
React
 ↓
.NET
 ↓
Python
 ↓
ML Model
```

Implementar flujo completo de predicción.

---

## Fase 8 — Reentrenamiento

* Nuevas ventas.
* Dataset actualizado.
* Entrenamiento.
* Evaluación.
* Comparación.
* Model promotion.

---

## Fase 9 — CI/CD

* GitHub Actions.
* Tests automáticos.
* Build automático.
* Validación del ML.
* Pipeline de deployment.

---

## Fase 10 — MLOps

Opcional inicialmente:

* MLflow.
* Experiment tracking.
* Model registry.
* Dataset versioning.
* Model metadata.

---

## Fase 11 — Docker

**Opcional**

* Dockerfiles.
* Docker Compose.
* Contenedores independientes.

---

## Fase 12 — Kubernetes

**Opcional**

* Deployment.
* Services.
* ConfigMaps.
* Secrets.
* Ingress.
* Scaling.

---

# 33. Principios del proyecto

El proyecto debe seguir estas reglas:

### 1. No utilizar IA generativa como sustituto de Machine Learning

La predicción de ventas debe realizarse mediante un modelo de ML entrenado con datos.

### 2. Comprender antes de abstraer

Antes de utilizar herramientas avanzadas como MLflow, se debe comprender:

* Dataset.
* Features.
* Training.
* Validation.
* Metrics.
* Model persistence.

### 3. Iteración

El proyecto debe construirse incrementalmente.

No implementar toda la arquitectura avanzada desde el principio.

### 4. Separación de responsabilidades

```text
React
→ UI

.NET
→ Business Logic / API

Python
→ Machine Learning

PostgreSQL
→ Persistence

GitHub Actions
→ CI/CD
```

### 5. Infraestructura opcional

Docker y Kubernetes nunca deben ser requisitos para ejecutar el proyecto durante las primeras fases.

---

# 34. Resultado esperado

Al finalizar las fases principales, el sistema deberá permitir:

```text
1. Registrar productos
          ↓
2. Registrar ventas
          ↓
3. Almacenar historial
          ↓
4. Analizar datos
          ↓
5. Entrenar modelo
          ↓
6. Evaluar modelo
          ↓
7. Registrar versión
          ↓
8. Generar predicciones
          ↓
9. Mostrar predicciones en React
          ↓
10. Recibir nuevas ventas
          ↓
11. Reentrenar
          ↓
12. Comparar modelos
          ↓
13. Activar modelo mejorado
```

El resultado final será una plataforma de ejemplo de **Machine Learning aplicado a un problema de negocio**, acompañada de prácticas de desarrollo profesional, testing, CI/CD y MLOps.

---

# 35. Posibles extensiones futuras

Una vez completado el sistema principal se podrán agregar:

* Predicción por hora.
* Predicción de inventario.
* Recomendación de producción.
* Predicción de ingredientes necesarios.
* Detección de anomalías en ventas.
* Identificación de productos con cambios inusuales de demanda.
* Incorporación de clima.
* Incorporación de días festivos.
* Incorporación de eventos locales.
* A/B testing de modelos.
* MLflow.
* Feature Store.
* Docker.
* Kubernetes.
* Cloud deployment.

Estas funcionalidades deben considerarse extensiones y no formar parte del MVP.

---

# 36. Definición del MVP

El MVP estará terminado cuando pueda realizarse correctamente el siguiente flujo:

```text
Usuario
  │
  ▼
Registra ventas
  │
  ▼
PostgreSQL
  │
  ▼
Dataset
  │
  ▼
Modelo ML
  │
  ▼
Predicción
  │
  ▼
Python API
  │
  ▼
.NET API
  │
  ▼
React Dashboard
```

Y el sistema deberá mostrar:

* Ventas históricas.
* Predicción futura.
* Modelo utilizado.
* Métricas del modelo.
* Fecha de entrenamiento.
* Versión del modelo.

---

# 37. Objetivo de aprendizaje personal

El proyecto debe utilizarse como una oportunidad para aprender de manera práctica:

### Machine Learning

* ¿Qué es un dataset?
* ¿Qué son features?
* ¿Cómo funciona el entrenamiento?
* ¿Qué es overfitting?
* ¿Cómo se evalúa un modelo?
* ¿Cómo elegir un modelo?
* ¿Cómo mejorar predicciones?

### MLOps

* Versionado de modelos.
* Experiment tracking.
* Model registry.
* Reentrenamiento.
* Model deployment.
* Model monitoring.

### CI/CD

* Pipelines.
* Automated testing.
* Build automation.
* Deployment automation.
* Quality gates.

### Software Engineering

* Arquitectura.
* Clean Architecture.
* CQRS.
* APIs.
* Testing.
* Seguridad.
* Separación de responsabilidades.

### Infraestructura

Opcional:

* Docker.
* Kubernetes.
* Cloud.
* Container orchestration.

---

# 38. Regla principal de desarrollo

> **No implementar una tecnología únicamente porque puede utilizarse. Cada componente debe existir porque resuelve una necesidad concreta del proyecto o porque representa un concepto de aprendizaje que se quiere experimentar.**

El proyecto debe priorizar el aprendizaje y la comprensión de Machine Learning, MLOps y CI/CD por encima de acumular tecnologías.
