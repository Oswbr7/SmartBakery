# 🤖 Smart Bakery — Machine Learning

## 1. Objetivo

El objetivo del componente de Machine Learning de Smart Bakery es desarrollar un modelo capaz de **predecir la cantidad de unidades que se venderán de un producto en una fecha determinada**.

Ejemplo:

```text
Producto:
Pastel de Chocolate

Fecha:
2026-10-15

Predicción:
18 unidades
```

El modelo deberá utilizar información histórica para identificar patrones relacionados con:

* Día de la semana.
* Mes.
* Temporada.
* Ventas anteriores.
* Tendencias recientes.
* Precio.
* Promociones.
* Producto.
* Categoría.
* Otros factores disponibles.

---

# 2. Problema de Machine Learning

El problema se define inicialmente como:

```text
Supervised Learning
        ↓
Regression
        ↓
Demand Forecasting
```

El modelo recibe un conjunto de características:

```text
Features
```

y produce:

```text
Target
```

En este proyecto:

```text
Features
    ↓
Información histórica + contexto de la venta

Target
    ↓
Cantidad de unidades vendidas
```

---

# 3. Definición del target

El target principal será:

```text
TargetQuantity
```

Representa:

> Cantidad de unidades de un producto vendidas durante una fecha determinada.

Ejemplo:

```text
Date        Product             Quantity
-----------------------------------------
2026-10-01  Chocolate Cake      15
2026-10-02  Chocolate Cake      18
2026-10-03  Chocolate Cake      24
```

El modelo aprenderá a aproximar:

```text
X → y
```

donde:

```text
X = features
y = TargetQuantity
```

---

# 4. Unidad de predicción

La unidad principal de predicción será:

```text
Producto + Fecha
```

Ejemplo:

```text
Chocolate Cake + 2026-10-15
```

Esto significa que el modelo no intentará predecir únicamente:

```text
"¿Cuántos productos venderá la panadería?"
```

sino:

```text
"¿Cuántas unidades del producto X se venderán el día Y?"
```

Esto permite generar posteriormente un pronóstico para múltiples productos.

---

# 5. Tipo de Machine Learning

El proyecto comenzará utilizando:

```text
Supervised Learning
```

porque disponemos de ejemplos históricos donde conocemos:

```text
Features → Actual Sales
```

Ejemplo:

```text
Día = Saturday
Precio = 350
Promoción = Yes
Ventas últimos 7 días = 92

                    ↓

             Actual Sales

                    ↓

                 27
```

El modelo utiliza muchos ejemplos de este tipo para aprender relaciones entre las variables y la cantidad vendida.

---

# 6. ¿Qué significa que el modelo "aprenda"?

El modelo no recibirá una regla escrita manualmente como:

```text
IF Saturday THEN sales = +20%
```

En lugar de eso recibirá muchos ejemplos históricos.

Por ejemplo:

```text
Monday     → 10
Tuesday    → 11
Wednesday  → 12
Thursday   → 13
Friday     → 18
Saturday   → 25
Sunday     → 22
```

El algoritmo buscará patrones estadísticos que permitan aproximar la relación entre las variables y el resultado.

Conceptualmente:

```text
Historical Data
       ↓
Learning Algorithm
       ↓
Model
       ↓
Prediction
```

---

# 7. Dataset

El dataset inicial será generado a partir de:

```text
PostgreSQL
```

utilizando:

```text
Sales
SaleItems
Products
Categories
Promotions
```

El dataset final será procesado por Python.

---

# 8. Dataset conceptual

Una fila representa:

```text
Producto + Fecha
```

Ejemplo:

```text
date        product_id   category    price   promotion   quantity
------------------------------------------------------------------
2026-09-01  cake-01      cakes       350     0           12
2026-09-02  cake-01      cakes       350     0           14
2026-09-03  cake-01      cakes       350     1           21
2026-09-04  cake-01      cakes       350     0           16
```

---

# 9. Features iniciales

Las primeras features serán relativamente sencillas.

## Temporal

```text
DayOfWeek
DayOfMonth
Month
WeekOfYear
Quarter
IsWeekend
```

---

## Producto

```text
ProductId
CategoryId
```

---

## Precio

```text
Price
```

---

## Promoción

```text
HasPromotion
DiscountPercentage
```

---

## Historial

```text
PreviousDaySales
AverageSales7Days
AverageSales14Days
AverageSales30Days
```

---

# 10. Feature engineering

Feature Engineering consiste en transformar los datos originales en variables que permitan al modelo identificar patrones útiles.

Por ejemplo, de:

```text
2026-10-10
```

podemos obtener:

```text
DayOfWeek = Saturday
DayOfMonth = 10
Month = 10
IsWeekend = true
```

El modelo no necesita interpretar una fecha como lo haría una persona.

En su lugar recibe variables numéricas o transformadas.

---

# 11. Variables temporales

Se crearán inicialmente:

```text
DayOfWeek
DayOfMonth
Month
WeekOfYear
Quarter
IsWeekend
```

Ejemplo:

```text
Date:
2026-10-10

DayOfWeek:
5

DayOfMonth:
10

Month:
10

IsWeekend:
1
```

Dependiendo del modelo, las variables categóricas podrán utilizarse mediante:

```text
One-Hot Encoding
```

u otra representación adecuada.

---

# 12. Variables históricas

Las ventas anteriores son algunas de las variables más importantes del proyecto.

Ejemplo:

```text
PreviousDaySales
```

representa:

```text
Ventas del mismo producto durante el día anterior.
```

También:

```text
AverageSales7Days
```

representa:

```text
Promedio de ventas del producto durante los últimos 7 días.
```

Y:

```text
AverageSales30Days
```

representa:

```text
Promedio de ventas del producto durante los últimos 30 días.
```

---

# 13. Lag features

Las variables históricas específicas se denominan frecuentemente:

```text
Lag Features
```

Ejemplo:

```text
Lag1
Lag7
Lag14
Lag30
```

Significado:

```text
Lag1  → ventas del día anterior
Lag7  → ventas de hace 7 días
Lag14 → ventas de hace 14 días
Lag30 → ventas de hace 30 días
```

Esto permite capturar patrones como:

```text
ventas similares cada semana
```

---

# 14. Rolling features

También se utilizarán promedios móviles.

Ejemplo:

```text
RollingMean7
RollingMean14
RollingMean30
```

Conceptualmente:

```text
RollingMean7 =
promedio de las ventas de los últimos 7 días
```

Estas variables ayudan a representar la tendencia reciente.

---

# 15. Evitar data leakage

Este será uno de los conceptos importantes del proyecto.

El modelo **no puede utilizar información del futuro para predecir el pasado**.

Incorrecto:

```text
Target:
Ventas del 10 de octubre

Feature:
Promedio que incluye ventas del 10 de octubre
```

Eso genera:

```text
Data Leakage
```

Correcto:

```text
Target:
Ventas del 10 de octubre

Feature:
Promedio calculado utilizando únicamente
información anterior al 10 de octubre.
```

Por ejemplo:

```text
Ventas 1-9 octubre
       ↓
Feature
       ↓
Predicción 10 octubre
```

---

# 16. Dataset final

El dataset podrá terminar teniendo una estructura similar a:

```text
Date
ProductId
CategoryId
Price
HasPromotion
DiscountPercentage
DayOfWeek
DayOfMonth
Month
WeekOfYear
IsWeekend
Lag1
Lag7
Lag14
Lag30
RollingMean7
RollingMean14
RollingMean30
TargetQuantity
```

---

# 17. Baseline

Antes de entrenar modelos complejos se debe crear un:

```text
Baseline
```

El baseline sirve como punto de comparación.

La primera estrategia puede ser:

```text
Prediction = AverageSalesLast7Days
```

Ejemplo:

```text
Últimos 7 días:
10
12
15
13
14
16
20

Promedio:
14.28
```

Predicción:

```text
14.28
```

Si un modelo sofisticado no puede mejorar consistentemente este resultado, probablemente no está aportando suficiente valor.

---

# 18. Primer modelo: Linear Regression

El primer modelo de Machine Learning será:

```text
Linear Regression
```

Es útil para comprender:

* Features.
* Target.
* Coeficientes.
* Entrenamiento.
* Predicción.
* Error.
* Overfitting.

Conceptualmente:

```text
Features
   ↓
Linear Regression
   ↓
Predicted Quantity
```

---

# 19. Segundo modelo: Random Forest

Después se utilizará:

```text
RandomForestRegressor
```

Random Forest permite aprender relaciones no lineales y combinaciones entre variables.

Ejemplo conceptual:

```text
Weekend
+
Promotion
+
High recent sales
        ↓
Higher predicted demand
```

No es necesario escribir explícitamente esa regla.

El modelo aprende patrones a partir de los datos.

---

# 20. Tercer modelo: Gradient Boosting

Posteriormente:

```text
GradientBoostingRegressor
```

Se utilizará para estudiar modelos basados en boosting.

La idea general es construir múltiples modelos secuencialmente donde cada etapa intenta mejorar los errores de las anteriores.

---

# 21. Modelos opcionales

Cuando el pipeline básico funcione correctamente podrán experimentarse:

```text
XGBoost
LightGBM
HistGradientBoostingRegressor
```

Estos modelos son opcionales.

No deben introducirse antes de comprender:

```text
Dataset
Features
Training
Validation
Testing
Metrics
Overfitting
```

---

# 22. Librerías

La implementación inicial utilizará:

```text
Python
Pandas
NumPy
scikit-learn
joblib
```

Opcionalmente:

```text
XGBoost
LightGBM
MLflow
```

---

# 23. Training Pipeline

El pipeline completo será:

```text
Raw Data
    ↓
Data Cleaning
    ↓
Feature Engineering
    ↓
Dataset Validation
    ↓
Train / Validation / Test
    ↓
Baseline
    ↓
Model Training
    ↓
Evaluation
    ↓
Model Selection
    ↓
Model Serialization
    ↓
Model Version
```

---

# 24. Data Cleaning

Antes de entrenar se deben validar:

```text
Missing values
Duplicate records
Invalid quantities
Invalid prices
Invalid dates
Unexpected categories
```

Ejemplo:

```text
Quantity = -5
```

debe considerarse inválido.

---

# 25. Missing Values

Se debe decidir cómo tratar valores faltantes.

Ejemplos:

```text
Price = NULL
Promotion = NULL
PreviousDaySales = NULL
```

Dependiendo de la variable se podrá:

```text
Eliminar
Imputar
Utilizar valor por defecto
```

La decisión debe documentarse.

No se debe rellenar automáticamente todo con `0` sin analizar el significado del dato.

---

# 26. Primeros registros

Las features históricas pueden generar valores faltantes naturalmente.

Ejemplo:

```text
Lag30
```

Para los primeros 30 días no existe información suficiente.

Esto debe tratarse explícitamente.

Opciones:

```text
Eliminar primeras filas
```

o:

```text
Utilizar una estrategia de imputación
```

Para el MVP se recomienda comenzar eliminando las filas que no tengan suficiente historial para calcular las features necesarias.

---

# 27. Train / Validation / Test

Como el problema está relacionado con fechas, no se debe hacer un split aleatorio convencional.

El dataset debe respetar el orden temporal.

Ejemplo:

```text
2024 ──────────────── 2025 ──────────────── 2026

|------ TRAIN -------|--- VALIDATION ---|-- TEST --|
```

Una división inicial podría ser:

```text
70% Train
15% Validation
15% Test
```

pero los porcentajes podrán cambiar dependiendo del volumen de datos.

Lo importante es:

```text
Pasado → Entrenamiento
Periodo posterior → Validación
Periodo más reciente → Test
```

---

# 28. ¿Por qué no hacer random split?

Un `random train_test_split` puede provocar que:

```text
Datos futuros
```

terminen dentro del entrenamiento mientras se intenta predecir:

```text
Datos pasados
```

Esto no representa correctamente el escenario real.

En forecasting debemos simular:

```text
Entrenar con pasado
        ↓
Predecir futuro
```

---

# 29. Validation

El conjunto de validación se utilizará para:

* Comparar modelos.
* Ajustar hiperparámetros.
* Evaluar feature engineering.
* Detectar overfitting.

Ejemplo:

```text
Model A
Validation MAE = 5.2

Model B
Validation MAE = 4.1
```

Se continuará investigando el modelo B, pero todavía no se debe utilizar el test final para tomar decisiones repetidamente.

---

# 30. Test

El conjunto de test representa datos que el proceso de desarrollo intenta mantener separados hasta la evaluación final.

Ejemplo:

```text
Train:
2024-01 → 2025-06

Validation:
2025-07 → 2025-12

Test:
2026-01 → 2026-06
```

El test proporciona una estimación más independiente del rendimiento del modelo sobre datos posteriores.

---

# 31. Cross-validation temporal

Para experimentos más avanzados se podrá utilizar:

```text
Time Series Cross Validation
```

Conceptualmente:

```text
Fold 1:
Train █████
Test       ██

Fold 2:
Train ███████
Test         ██

Fold 3:
Train █████████
Test           ██
```

Esto permite evaluar el comportamiento del modelo en múltiples periodos temporales.

---

# 32. Métrica principal: MAE

La métrica principal será:

```text
MAE
```

Mean Absolute Error.

Conceptualmente:

```text
MAE =
promedio de |real - predicción|
```

Ejemplo:

```text
Real:        20
Predicción:  17

Error:        3
```

Si:

```text
MAE = 3
```

significa que, en promedio, las predicciones se alejan aproximadamente 3 unidades del valor real.

---

# 33. RMSE

También se utilizará:

```text
RMSE
```

Root Mean Squared Error.

RMSE penaliza más fuertemente errores grandes.

Esto puede ser útil para detectar modelos que ocasionalmente realizan predicciones muy alejadas del valor real.

---

# 34. R²

También se podrá registrar:

```text
R²
```

R² proporciona información sobre cuánto de la variabilidad del target está siendo explicada por el modelo bajo la definición de esta métrica.

No debe utilizarse como única métrica para decidir si un modelo de forecasting es útil.

---

# 35. Métricas iniciales

Cada entrenamiento deberá registrar:

```text
MAE
RMSE
R²
```

Ejemplo:

```text
Model:
RandomForest

MAE:
3.82

RMSE:
5.71

R²:
0.81
```

---

# 36. Métricas por producto

Una mejora importante será evaluar el modelo globalmente y también por producto.

Ejemplo:

```text
Overall MAE = 4.2
```

Pero:

```text
Chocolate Cake = 2.1
Cheesecake      = 3.4
Cupcake         = 6.8
```

Esto puede revelar que el modelo funciona de forma diferente dependiendo del producto.

---

# 37. Error relativo

También podrá estudiarse:

```text
MAE / Average Demand
```

porque un error de:

```text
5 unidades
```

no significa lo mismo para:

```text
Producto A:
10 unidades/día
```

que para:

```text
Producto B:
100 unidades/día
```

Esta métrica puede incorporarse posteriormente.

---

# 38. Overfitting

Uno de los conceptos que se debe estudiar durante el proyecto es:

```text
Overfitting
```

Ocurre cuando el modelo aprende demasiado bien los datos de entrenamiento pero no generaliza adecuadamente a datos nuevos.

Ejemplo:

```text
Train MAE:
0.8

Validation MAE:
7.2
```

Esto puede ser una señal de overfitting.

---

# 39. Underfitting

El caso contrario:

```text
Train MAE:
8.0

Validation MAE:
8.3
```

puede indicar que el modelo todavía no está capturando suficientemente los patrones existentes.

Esto debe analizarse junto con el contexto del dataset y la complejidad del problema.

---

# 40. Hyperparameters

Los modelos tienen parámetros que controlan su comportamiento.

Ejemplo:

```text
Random Forest

n_estimators
max_depth
min_samples_split
min_samples_leaf
```

El objetivo será aprender posteriormente cómo ajustar estos valores.

Inicialmente se utilizarán configuraciones sencillas.

---

# 41. Feature Engineering experiments

El proyecto debe permitir experimentar con diferentes conjuntos de features.

Ejemplo:

### Experiment A

```text
DayOfWeek
Price
Promotion
```

### Experiment B

```text
DayOfWeek
Price
Promotion
Lag1
Lag7
RollingMean7
```

### Experiment C

```text
DayOfWeek
Month
Price
Promotion
Lag1
Lag7
Lag14
Lag30
RollingMean7
RollingMean30
```

Después se compararán las métricas.

---

# 42. Experiment tracking

Cada experimento deberá registrar:

```text
Experiment name
Date
Dataset version
Features
Algorithm
Hyperparameters
MAE
RMSE
R²
Notes
```

Inicialmente puede guardarse como:

```text
JSON
```

o mediante archivos estructurados.

Posteriormente puede utilizarse:

```text
MLflow
```

---

# 43. Model serialization

Una vez entrenado un modelo satisfactorio se deberá serializar.

Inicialmente:

```text
joblib
```

Ejemplo:

```text
models/
├── model_v1.joblib
├── model_v2.joblib
└── model_v3.joblib
```

El modelo almacenado debe poder cargarse posteriormente para realizar predicciones sin necesidad de volver a entrenarlo.

---

# 44. Model versioning

Cada modelo tendrá una versión.

Ejemplo:

```text
v1
v2
v3
```

La versión debe estar relacionada con:

```text
Algorithm
DatasetVersion
Features
Metrics
TrainingDate
ModelPath
```

Ejemplo:

```text
Model v3

Algorithm:
RandomForest

Dataset:
sales_2026_09

Features:
18

MAE:
3.91

RMSE:
5.42

R²:
0.83
```

---

# 45. Candidate model

Un modelo recién entrenado no debe convertirse automáticamente en producción.

Flujo:

```text
Training
    ↓
Candidate
    ↓
Evaluation
    ↓
Comparison
    ↓
Approved / Rejected
```

---

# 46. Production model

El modelo utilizado para las predicciones será:

```text
Production Model
```

Ejemplo:

```text
Current Production Model:
v3
```

Un nuevo modelo:

```text
v4
```

debe evaluarse antes de reemplazarlo.

---

# 47. Modelo de promoción

Inicialmente se utilizará una regla sencilla:

```text
Si el nuevo modelo mejora la métrica principal
y cumple los criterios mínimos,
puede convertirse en Production.
```

Para MAE:

```text
MAE_new < MAE_production
```

La decisión también deberá considerar:

* Mismo conjunto de evaluación.
* Ausencia de errores críticos.
* Métricas secundarias.
* Calidad de los datos.
* Estabilidad del entrenamiento.

---

# 48. Retraining

El modelo deberá poder reentrenarse cuando existan nuevos datos.

Flujo:

```text
New Sales
    ↓
Dataset Update
    ↓
Feature Engineering
    ↓
Training
    ↓
Evaluation
    ↓
Candidate Model
    ↓
Comparison
    ↓
Production / Rejected
```

---

# 49. Continuous Learning

En Smart Bakery, "continuous learning" inicialmente significará:

```text
Periodic Retraining
```

No se implementará inicialmente:

```text
Online Learning
```

Es decir, el modelo no estará modificando sus parámetros después de cada venta.

En su lugar:

```text
Nuevos datos
     ↓
Nuevo entrenamiento
     ↓
Nuevo modelo
```

---

# 50. Retraining schedule

Inicialmente podrá ejecutarse manualmente:

```text
POST /train
```

Posteriormente:

```text
Weekly
```

o:

```text
Monthly
```

dependiendo de la cantidad de datos.

En una versión avanzada, GitHub Actions u otro scheduler podrá ejecutar el pipeline automáticamente.

---

# 51. Prediction pipeline

Para generar una predicción:

```text
Product
+
Prediction Date
+
Historical Data
        ↓
Feature Engineering
        ↓
Production Model
        ↓
Prediction
```

Ejemplo:

```text
Product:
Chocolate Cake

Date:
2026-10-15

Features:
Lag1 = 17
Lag7 = 21
RollingMean7 = 18.7
Promotion = 1
Weekend = 0

                ↓

Production Model v3

                ↓

Prediction = 20.4
```

---

# 52. Prediction post-processing

El modelo puede devolver:

```text
20.4
```

Pero no se pueden vender:

```text
0.4 unidades de pastel
```

Por lo tanto, la aplicación puede transformar el resultado:

```text
20.4 → 20
```

o:

```text
20.4 → 21
```

La estrategia exacta debe definirse como una regla de negocio independiente del modelo.

El modelo no debe asumir directamente las reglas de inventario o producción.

---

# 53. Confidence / uncertainty

En una etapa posterior se podrá estudiar la incertidumbre de las predicciones.

Ejemplo conceptual:

```text
Prediction:
20

Prediction Interval:
17 - 24
```

Esto puede ser más útil para planificación que un único número.

No es necesario implementar prediction intervals en el MVP.

---

# 54. Feature importance

Para modelos que permitan calcular importancia de variables, se podrá analizar:

```text
Feature
Importance
```

Ejemplo conceptual:

```text
RollingMean7       0.31
Lag7               0.21
Promotion          0.17
DayOfWeek          0.14
Price              0.09
Month              0.08
```

Estas cifras son únicamente un ejemplo.

No deben asumirse como resultados reales del proyecto.

---

# 55. Interpretabilidad

El sistema debe permitir investigar por qué un modelo produce determinados resultados.

Dependiendo del algoritmo podrán estudiarse:

```text
Feature Importance
Permutation Importance
SHAP
```

SHAP podrá agregarse posteriormente como una herramienta avanzada.

---

# 56. Model limitations

El modelo tendrá limitaciones.

Por ejemplo, inicialmente podría no conocer:

```text
Weather
Local Events
Competitor Prices
Unexpected Holidays
Supply Problems
Store Closures
Viral Trends
```

Por lo tanto:

```text
Prediction ≠ Certainty
```

Las predicciones deben considerarse estimaciones basadas en los datos disponibles.

---

# 57. Synthetic data limitations

Los datos sintéticos deben utilizarse principalmente para:

```text
Desarrollo
Pruebas
Experimentación
Aprendizaje
```

Un modelo entrenado únicamente con datos sintéticos no debe considerarse automáticamente validado para un negocio real.

Cuando existan suficientes datos reales, deberán utilizarse para evaluar el comportamiento real del modelo.

---

# 58. ML Service

El Machine Learning se ejecutará mediante:

```text
Python
FastAPI
```

El servicio tendrá endpoints como:

```text
GET  /health
POST /predict
POST /train
GET  /model
GET  /metrics
```

---

# 59. /predict

Entrada conceptual:

```json
{
  "productId": "cake-01",
  "date": "2026-10-15"
}
```

Respuesta conceptual:

```json
{
  "prediction": 20.4,
  "modelVersion": "v3"
}
```

---

# 60. /train

El endpoint de entrenamiento podrá recibir información sobre el dataset o utilizar la fuente configurada.

Conceptualmente:

```text
POST /train
```

Resultado:

```json
{
  "modelVersion": "v4",
  "mae": 3.72,
  "rmse": 5.11,
  "r2": 0.85,
  "status": "candidate"
}
```

---

# 61. Separación de responsabilidades

Python será responsable de:

```text
✓ Dataset processing
✓ Feature engineering
✓ Training
✓ Evaluation
✓ Prediction
✓ Model serialization
✓ Model loading
```

Python no será responsable de:

```text
✗ Authentication
✗ User management
✗ Product CRUD
✗ Sales CRUD
✗ Bakery business rules
```

Estas responsabilidades pertenecen al backend .NET.

---

# 62. ML project structure

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
│   │   ├── preprocessing.py
│   │   ├── features.py
│   │   ├── training.py
│   │   ├── evaluation.py
│   │   └── prediction.py
│   │
│   └── services/
│
├── models/
│
├── datasets/
│
├── experiments/
│
├── tests/
│
├── requirements.txt
└── README.md
```

---

# 63. ML tests

El ML service deberá incluir pruebas para:

### Data processing

```text
✓ Missing values
✓ Invalid values
✓ Feature generation
```

### Feature engineering

```text
✓ Lag calculations
✓ Rolling averages
✓ Temporal features
```

### Training

```text
✓ Model can train
✓ Model produces metrics
✓ Model can be serialized
```

### Prediction

```text
✓ Model loads
✓ Prediction returns valid value
✓ Prediction is non-negative
```

---

# 64. Reproducibility

Los experimentos deben ser reproducibles siempre que sea posible.

Se deberá registrar:

```text
Random seed
Dataset version
Algorithm
Hyperparameters
Features
Python version
Library versions
```

Ejemplo:

```text
RandomSeed = 42
```

Esto permite repetir experimentos y comparar resultados de forma más confiable.

---

# 65. Model artifact validation

Antes de considerar un modelo válido:

```text
Model file exists
Model can be loaded
Expected features exist
Prediction succeeds
Metrics are available
```

Si alguna validación falla:

```text
Model = Invalid
```

y no deberá promocionarse.

---

# 66. Data validation

Antes del entrenamiento:

```text
Dataset exists
    ↓
Rows > minimum threshold
    ↓
Required columns exist
    ↓
No invalid target values
    ↓
Dates valid
    ↓
Features generated
    ↓
Training allowed
```

Esto evita entrenar accidentalmente con datasets incompletos.

---

# 67. Minimum dataset

El proyecto debe definir una cantidad mínima de datos antes de entrenar.

Ejemplo conceptual:

```text
Minimum:
Several months of historical data
```

La cantidad exacta dependerá de:

* Número de productos.
* Frecuencia de ventas.
* Estacionalidad.
* Complejidad del modelo.

El sistema no debe asumir que cualquier cantidad de filas es suficiente para un modelo útil.

---

# 68. ML development stages

## Stage 1 — Understanding

Aprender:

```text
Supervised Learning
Regression
Features
Target
Dataset
Training
Prediction
```

---

## Stage 2 — First model

Implementar:

```text
Baseline
Linear Regression
```

---

## Stage 3 — Better models

Implementar:

```text
Random Forest
Gradient Boosting
```

---

## Stage 4 — Feature engineering

Agregar:

```text
Lag features
Rolling features
Promotions
Seasonality
```

---

## Stage 5 — Evaluation

Implementar:

```text
MAE
RMSE
R²
Temporal validation
```

---

## Stage 6 — Model versioning

Implementar:

```text
ModelVersion
Model artifacts
Candidate
Production
Rejected
Retired
```

---

## Stage 7 — MLOps

Agregar:

```text
Experiment tracking
Automatic retraining
Model monitoring
Dataset versioning
MLflow
```

---

# 69. Learning checklist

El desarrollo del proyecto debe permitir aprender:

```text
[ ] ¿Qué es Machine Learning?
[ ] ¿Qué es Supervised Learning?
[ ] ¿Qué es Regression?
[ ] ¿Qué es un dataset?
[ ] ¿Qué es una feature?
[ ] ¿Qué es un target?
[ ] ¿Qué es training?
[ ] ¿Qué es validation?
[ ] ¿Qué es testing?
[ ] ¿Qué es overfitting?
[ ] ¿Qué es underfitting?
[ ] ¿Qué es data leakage?
[ ] ¿Qué es feature engineering?
[ ] ¿Qué son lag features?
[ ] ¿Qué son rolling features?
[ ] ¿Qué es un baseline?
[ ] ¿Qué es MAE?
[ ] ¿Qué es RMSE?
[ ] ¿Qué es R²?
[ ] ¿Qué son hiperparámetros?
[ ] ¿Qué es model versioning?
[ ] ¿Qué es model serialization?
[ ] ¿Qué es retraining?
[ ] ¿Qué es MLOps?
```

---

# 70. MVP ML

El MVP debe conseguir:

```text
PostgreSQL
      ↓
Historical Sales
      ↓
Python
      ↓
Dataset
      ↓
Feature Engineering
      ↓
Baseline
      ↓
Linear Regression
      ↓
Random Forest
      ↓
Evaluation
      ↓
Best Candidate
      ↓
joblib
      ↓
FastAPI
      ↓
Prediction
```

---

# 71. Definition of Done

La primera versión de Machine Learning estará terminada cuando:

```text
[ ] Existe dataset histórico.
[ ] El dataset puede generarse reproduciblemente.
[ ] Las features se generan automáticamente.
[ ] No existe data leakage conocido.
[ ] Existe un baseline.
[ ] Existe al menos un modelo de regresión.
[ ] Existe un segundo modelo para comparación.
[ ] Se utiliza validación temporal.
[ ] Se calculan MAE, RMSE y R².
[ ] Los experimentos pueden compararse.
[ ] El modelo puede serializarse.
[ ] El modelo puede cargarse posteriormente.
[ ] Existe una versión de modelo.
[ ] FastAPI puede realizar predicciones.
[ ] Las predicciones se pueden guardar en PostgreSQL.
[ ] El backend .NET puede consumir el ML service.
```

---

# 72. Arquitectura final

```text
                         ┌──────────────────┐
                         │    PostgreSQL    │
                         │                  │
                         │ Sales            │
                         │ Products         │
                         │ Promotions       │
                         └────────┬─────────┘
                                  │
                                  │ Historical Data
                                  ▼
                         ┌──────────────────┐
                         │  Python ML       │
                         │                  │
                         │ Preprocessing     │
                         │ Feature Engineer  │
                         │ Training          │
                         │ Evaluation        │
                         │ Prediction        │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │  Model Artifact  │
                         │    .joblib       │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │    FastAPI       │
                         │   ML Service     │
                         └────────┬─────────┘
                                  │
                                  │ Prediction
                                  ▼
                         ┌──────────────────┐
                         │    .NET API      │
                         └────────┬─────────┘
                                  │
                                  ▼
                         ┌──────────────────┐
                         │      React       │
                         │    Dashboard     │
                         └──────────────────┘
```

---

# 73. Core principle

> **Primero debemos conseguir un pipeline de Machine Learning correcto y reproducible; después debemos intentar hacerlo más sofisticado.**

El objetivo del proyecto no es simplemente utilizar el algoritmo más avanzado disponible.

El objetivo es comprender y construir correctamente:

```text
Data
  ↓
Features
  ↓
Training
  ↓
Validation
  ↓
Evaluation
  ↓
Model
  ↓
Prediction
  ↓
Monitoring
  ↓
Retraining
```

Este ciclo constituye la base del componente de Machine Learning y posteriormente permitirá introducir conceptos de MLOps.
