# 🗄️ Smart Bakery — Database Design

## 1. Objetivo

Este documento define el diseño de la base de datos de **Smart Bakery**.

La base de datos debe soportar:

* Gestión de usuarios.
* Gestión de productos.
* Categorías de productos.
* Promociones.
* Registro de ventas.
* Detalles de ventas.
* Predicciones de Machine Learning.
* Versionado de modelos.
* Métricas de modelos.
* Historial suficiente para construir datasets de Machine Learning.

La base de datos principal será **PostgreSQL**.

---

# 2. Principios de diseño

La base de datos debe seguir los siguientes principios:

1. Mantener separación clara entre datos de negocio y datos de Machine Learning.
2. Evitar duplicación innecesaria de información.
3. Mantener historial de ventas inmutable cuando sea posible.
4. No almacenar directamente los modelos ML dentro de PostgreSQL.
5. Permitir reconstruir datasets históricos.
6. Utilizar relaciones mediante Foreign Keys.
7. Utilizar índices donde exista un patrón claro de consulta.
8. Mantener timestamps para auditoría.
9. Evitar eliminar físicamente información importante del historial.
10. Diseñar el esquema para permitir crecimiento futuro.

---

# 3. Motor

```text
Database:
PostgreSQL

ORM:
Entity Framework Core

Migrations:
EF Core Migrations
```

---

# 4. Diagrama general

```text
┌──────────────┐
│    Users     │
└──────────────┘
       │
       │
       ▼
┌──────────────┐
│    Roles     │
└──────────────┘


┌──────────────┐       ┌────────────────┐
│  Categories  │──────►│    Products    │
└──────────────┘       └───────┬────────┘
                               │
                               │
                               ▼
                        ┌──────────────┐
                        │  SaleItems   │
                        └──────┬───────┘
                               │
                               ▼
                        ┌──────────────┐
                        │    Sales     │
                        └──────┬───────┘
                               │
                               ▼
                        ┌──────────────┐
                        │   Dataset    │
                        │  generation  │
                        └──────────────┘


┌──────────────┐       ┌──────────────────┐
│   Products   │──────►│   Predictions    │
└──────────────┘       └────────┬─────────┘
                                │
                                ▼
                       ┌──────────────────┐
                       │ ModelVersions    │
                       └──────────────────┘
```

---

# 5. Entidades

Las entidades principales serán:

```text
User
Role
Category
Product
Promotion
Sale
SaleItem
SalePromotion
Prediction
ModelVersion
ModelMetric
```

---

# 6. Users

Representa a los usuarios de la aplicación.

### Tabla

```text
users
```

### Campos

| Campo         | Tipo         | Restricciones    |
| ------------- | ------------ | ---------------- |
| id            | UUID         | PK               |
| username      | VARCHAR(100) | UNIQUE, NOT NULL |
| email         | VARCHAR(255) | UNIQUE, NOT NULL |
| password_hash | TEXT         | NOT NULL         |
| role_id       | UUID         | FK               |
| is_active     | BOOLEAN      | NOT NULL         |
| created_at    | TIMESTAMP    | NOT NULL         |
| updated_at    | TIMESTAMP    | NOT NULL         |

### Notas

Nunca almacenar contraseñas en texto plano.

La contraseña debe almacenarse utilizando un algoritmo seguro de hashing.

---

# 7. Roles

Representa los permisos generales del usuario.

### Tabla

```text
roles
```

### Campos

| Campo       | Tipo         | Restricciones    |
| ----------- | ------------ | ---------------- |
| id          | UUID         | PK               |
| name        | VARCHAR(50)  | UNIQUE, NOT NULL |
| description | VARCHAR(255) | NULL             |

Valores iniciales:

```text
Admin
Employee
```

---

# 8. Categories

Permite clasificar los productos.

### Tabla

```text
categories
```

### Campos

| Campo       | Tipo         | Restricciones    |
| ----------- | ------------ | ---------------- |
| id          | UUID         | PK               |
| name        | VARCHAR(100) | UNIQUE, NOT NULL |
| description | TEXT         | NULL             |
| is_active   | BOOLEAN      | NOT NULL         |
| created_at  | TIMESTAMP    | NOT NULL         |
| updated_at  | TIMESTAMP    | NOT NULL         |

Ejemplos:

```text
Pasteles
Cheesecakes
Cupcakes
Galletas
Postres
Bebidas
```

---

# 9. Products

Representa los productos vendidos por la pastelería.

### Tabla

```text
products
```

### Campos

| Campo       | Tipo          | Restricciones |
| ----------- | ------------- | ------------- |
| id          | UUID          | PK            |
| category_id | UUID          | FK, NOT NULL  |
| name        | VARCHAR(150)  | NOT NULL      |
| description | TEXT          | NULL          |
| price       | NUMERIC(10,2) | NOT NULL      |
| is_active   | BOOLEAN       | NOT NULL      |
| created_at  | TIMESTAMP     | NOT NULL      |
| updated_at  | TIMESTAMP     | NOT NULL      |

### Reglas

`price` debe ser mayor que `0`.

Un producto vendido históricamente no debe eliminarse físicamente.

Debe utilizarse:

```text
is_active = false
```

cuando deje de venderse.

---

# 10. Product Price History

Para análisis históricos, el precio actual del producto no es suficiente.

Un cambio de precio puede afectar las ventas y debe poder reconstruirse posteriormente.

### Tabla

```text
product_price_history
```

### Campos

| Campo          | Tipo          | Restricciones |
| -------------- | ------------- | ------------- |
| id             | UUID          | PK            |
| product_id     | UUID          | FK, NOT NULL  |
| price          | NUMERIC(10,2) | NOT NULL      |
| effective_from | TIMESTAMP     | NOT NULL      |
| effective_to   | TIMESTAMP     | NULL          |
| created_at     | TIMESTAMP     | NOT NULL      |

Ejemplo:

```text
Chocolate
---------
$300 → Enero
$320 → Marzo
$350 → Julio
```

Esto permitirá al modelo conocer el precio vigente cuando ocurrió una venta.

---

# 11. Promotions

Representa promociones aplicadas a productos o ventas.

### Tabla

```text
promotions
```

### Campos

| Campo               | Tipo         | Restricciones |
| ------------------- | ------------ | ------------- |
| id                  | UUID         | PK            |
| name                | VARCHAR(150) | NOT NULL      |
| description         | TEXT         | NULL          |
| discount_percentage | NUMERIC(5,2) | NOT NULL      |
| start_date          | DATE         | NOT NULL      |
| end_date            | DATE         | NOT NULL      |
| is_active           | BOOLEAN      | NOT NULL      |
| created_at          | TIMESTAMP    | NOT NULL      |

Ejemplos:

```text
10% cumpleaños
20% fin de semana
2x1 cupcakes
Promoción Navidad
```

---

# 12. Promotion Products

Una promoción puede aplicarse a múltiples productos.

### Tabla

```text
promotion_products
```

### Campos

| Campo        | Tipo | Restricciones |
| ------------ | ---- | ------------- |
| promotion_id | UUID | FK            |
| product_id   | UUID | FK            |

Primary Key:

```text
(promotion_id, product_id)
```

Relación:

```text
Promotion
   │
   ├── Product A
   ├── Product B
   └── Product C
```

---

# 13. Sales

Representa una venta completa.

### Tabla

```text
sales
```

### Campos

| Campo           | Tipo          | Restricciones |
| --------------- | ------------- | ------------- |
| id              | UUID          | PK            |
| sale_date       | TIMESTAMP     | NOT NULL      |
| user_id         | UUID          | FK            |
| subtotal        | NUMERIC(12,2) | NOT NULL      |
| discount_amount | NUMERIC(12,2) | NOT NULL      |
| total_amount    | NUMERIC(12,2) | NOT NULL      |
| created_at      | TIMESTAMP     | NOT NULL      |

### Importante

La venta debe conservar los valores monetarios correspondientes al momento de la transacción.

No se debe recalcular una venta histórica utilizando el precio actual del producto.

---

# 14. SaleItems

Representa los productos incluidos en una venta.

### Tabla

```text
sale_items
```

### Campos

| Campo           | Tipo          | Restricciones |
| --------------- | ------------- | ------------- |
| id              | UUID          | PK            |
| sale_id         | UUID          | FK, NOT NULL  |
| product_id      | UUID          | FK, NOT NULL  |
| quantity        | INTEGER       | NOT NULL      |
| unit_price      | NUMERIC(10,2) | NOT NULL      |
| discount_amount | NUMERIC(10,2) | NOT NULL      |
| subtotal        | NUMERIC(12,2) | NOT NULL      |

### Ejemplo

```text
Sale
--------------------------------
Chocolate x2     $350 = $700
Fresa x1         $300 = $300
--------------------------------
Subtotal                 $1000
Descuento                  $100
Total                      $900
```

### Regla importante

`unit_price` debe representar el precio utilizado durante esa venta.

No debe depender del precio actual de `products`.

---

# 15. Sale Promotions

Permite registrar qué promociones fueron utilizadas en una venta.

### Tabla

```text
sale_promotions
```

### Campos

| Campo           | Tipo          | Restricciones |
| --------------- | ------------- | ------------- |
| sale_id         | UUID          | FK            |
| promotion_id    | UUID          | FK            |
| discount_amount | NUMERIC(10,2) | NOT NULL      |

Primary Key:

```text
(sale_id, promotion_id)
```

Esto permitirá analizar posteriormente:

> ¿Las promociones realmente aumentan las ventas?

---

# 16. Predictions

Representa una predicción realizada por el modelo.

### Tabla

```text
predictions
```

### Campos

| Campo              | Tipo          | Restricciones |
| ------------------ | ------------- | ------------- |
| id                 | UUID          | PK            |
| product_id         | UUID          | FK            |
| prediction_date    | DATE          | NOT NULL      |
| predicted_quantity | NUMERIC(10,2) | NOT NULL      |
| model_version_id   | UUID          | FK            |
| created_at         | TIMESTAMP     | NOT NULL      |

---

# 17. Actual Sales vs Predictions

Cuando llegue la fecha de una predicción, debe ser posible comparar:

```text
Prediction
     │
     ▼
predicted_quantity = 17
     │
     │
     ▼
Actual Sale
     │
     ▼
actual_quantity = 15
```

No es necesario almacenar obligatoriamente `actual_quantity` dentro de `predictions`.

Puede calcularse a partir de `sale_items`.

Esto evita duplicar información.

---

# 18. ModelVersions

Representa una versión de un modelo entrenado.

### Tabla

```text
model_versions
```

### Campos

| Campo           | Tipo         | Restricciones |
| --------------- | ------------ | ------------- |
| id              | UUID         | PK            |
| version         | VARCHAR(50)  | NOT NULL      |
| algorithm       | VARCHAR(100) | NOT NULL      |
| dataset_version | VARCHAR(100) | NOT NULL      |
| model_path      | TEXT         | NOT NULL      |
| status          | VARCHAR(30)  | NOT NULL      |
| training_date   | TIMESTAMP    | NOT NULL      |
| is_production   | BOOLEAN      | NOT NULL      |
| created_at      | TIMESTAMP    | NOT NULL      |

Estados:

```text
Candidate
Production
Rejected
Retired
```

---

# 19. ModelMetrics

Permite almacenar las métricas obtenidas durante el entrenamiento.

### Tabla

```text
model_metrics
```

### Campos

| Campo            | Tipo          | Restricciones |
| ---------------- | ------------- | ------------- |
| id               | UUID          | PK            |
| model_version_id | UUID          | FK            |
| mae              | NUMERIC(12,6) | NOT NULL      |
| rmse             | NUMERIC(12,6) | NOT NULL      |
| r2               | NUMERIC(12,6) | NOT NULL      |
| dataset_size     | INTEGER       | NOT NULL      |
| created_at       | TIMESTAMP     | NOT NULL      |

Relación:

```text
ModelVersion
      │
      ├── Metric
      ├── Metric
      └── Metric
```

Inicialmente puede existir una sola fila de métricas por modelo.

La estructura permitirá almacenar más evaluaciones posteriormente si es necesario.

---

# 20. Dataset Versions

El dataset utilizado para entrenar un modelo también debe tener versión.

### Tabla

```text
dataset_versions
```

### Campos

| Campo      | Tipo         | Restricciones    |
| ---------- | ------------ | ---------------- |
| id         | UUID         | PK               |
| version    | VARCHAR(50)  | UNIQUE, NOT NULL |
| source     | VARCHAR(100) | NOT NULL         |
| row_count  | INTEGER      | NOT NULL         |
| start_date | DATE         | NOT NULL         |
| end_date   | DATE         | NOT NULL         |
| created_at | TIMESTAMP    | NOT NULL         |

Ejemplo:

```text
Dataset v1
01/01/2025 → 31/12/2025
3650 registros
```

---

# 21. Relación Dataset → Model

Un modelo debe registrar qué dataset utilizó.

```text
DatasetVersion
      │
      │ 1:N
      ▼
ModelVersion
```

Ejemplo:

```text
Dataset v4
    │
    ├── Model v7
    ├── Model v8
    └── Model v9
```

Esto permite reproducir experimentos.

---

# 22. Entity Relationship Diagram

Vista simplificada:

```text
┌──────────────┐
│    roles     │
└──────┬───────┘
       │ 1:N
       ▼
┌──────────────┐
│    users     │
└──────┬───────┘
       │
       │ 1:N
       ▼
┌──────────────┐
│    sales     │
└──────┬───────┘
       │ 1:N
       ▼
┌──────────────┐       ┌──────────────┐
│  sale_items  │──────►│   products   │
└──────────────┘  N:1  └──────┬───────┘
                              │
                              │ N:1
                              ▼
                       ┌──────────────┐
                       │  categories  │
                       └──────────────┘


┌──────────────┐       ┌────────────────────┐
│ promotions   │◄─────►│ promotion_products │
└──────┬───────┘       └─────────┬──────────┘
       │                          │
       ▼                          ▼
┌──────────────┐             ┌──────────────┐
│sale_promotions│            │   products   │
└──────────────┘             └──────────────┘


┌──────────────────┐
│ dataset_versions │
└────────┬─────────┘
         │ 1:N
         ▼
┌──────────────────┐
│  model_versions  │
└───────┬──────────┘
        │ 1:N
        ├──────────────────┐
        ▼                  ▼
┌────────────────┐  ┌──────────────┐
│ model_metrics  │  │ predictions  │
└────────────────┘  └──────┬───────┘
                           │
                           ▼
                      ┌────────────┐
                      │  products  │
                      └────────────┘
```

---

# 23. Relaciones principales

## User → Role

```text
Role 1 ─── N Users
```

Un rol puede pertenecer a múltiples usuarios.

---

## Category → Product

```text
Category 1 ─── N Products
```

Una categoría puede contener múltiples productos.

---

## Product → SaleItem

```text
Product 1 ─── N SaleItems
```

Un producto puede aparecer en múltiples ventas.

---

## Sale → SaleItem

```text
Sale 1 ─── N SaleItems
```

Una venta puede contener múltiples productos.

---

## Product → Prediction

```text
Product 1 ─── N Predictions
```

Un producto puede tener múltiples predicciones históricas.

---

## DatasetVersion → ModelVersion

```text
DatasetVersion 1 ─── N ModelVersions
```

Un dataset puede utilizarse para entrenar múltiples modelos.

---

## ModelVersion → ModelMetrics

```text
ModelVersion 1 ─── N ModelMetrics
```

Una versión puede tener múltiples evaluaciones.

---

# 24. Identificadores

Las entidades principales utilizarán UUID.

Ejemplo:

```text
550e8400-e29b-41d4-a716-446655440000
```

Motivos:

* Evita depender de IDs secuenciales.
* Facilita integración futura.
* Evita colisiones entre sistemas.
* Es apropiado para APIs distribuidas.

Para tablas de relación simples se pueden utilizar claves compuestas.

---

# 25. Timestamps

Las entidades principales deberán utilizar:

```text
created_at
updated_at
```

Cuando tenga sentido.

Las fechas de negocio deberán distinguirse de los timestamps de auditoría.

Ejemplo:

```text
sale_date
created_at
```

No representan lo mismo.

`created_at` indica cuándo se registró la venta en el sistema.

`sale_date` indica cuándo ocurrió la venta.

---

# 26. Soft Delete

No se recomienda eliminar físicamente:

* Productos que tengan ventas.
* Usuarios con historial.
* Categorías con productos históricos.

Se utilizará:

```text
is_active = false
```

cuando sea apropiado.

Las ventas históricas nunca deben eliminarse como mecanismo normal de administración.

---

# 27. Integridad de ventas

Una venta debe cumplir:

```text
subtotal >= 0
discount_amount >= 0
total_amount >= 0
```

Y:

```text
total_amount = subtotal - discount_amount
```

Los valores finales deben ser calculados y validados por el backend.

El frontend nunca debe ser considerado una fuente confiable para los cálculos financieros.

---

# 28. Integridad de SaleItems

Cada `SaleItem` debe cumplir:

```text
quantity > 0
unit_price >= 0
discount_amount >= 0
subtotal >= 0
```

El subtotal debe derivarse de:

```text
quantity × unit_price - discount
```

La API debe validar estos valores.

---

# 29. Índices

Se deben crear índices para las consultas más frecuentes.

Inicialmente:

```text
products
---------
category_id
is_active
name


sales
---------
sale_date
user_id


sale_items
---------
sale_id
product_id


predictions
-----------
product_id
prediction_date
model_version_id


model_versions
--------------
status
is_production
training_date


dataset_versions
----------------
version
```

No se deben crear índices indiscriminadamente.

Los índices adicionales se agregarán basándose en consultas reales y profiling.

---

# 30. Constraints

Se deberán utilizar constraints de PostgreSQL cuando sea apropiado.

Ejemplos:

```text
products.price > 0

sale_items.quantity > 0

promotions.discount_percentage >= 0

promotions.discount_percentage <= 100

sales.total_amount >= 0
```

Las reglas críticas de integridad deben existir tanto en la capa de aplicación como, cuando sea apropiado, en la base de datos.

---

# 31. Machine Learning Dataset

El dataset de ML no debe ser una tabla completamente separada con todos los datos duplicados.

Inicialmente se generará mediante consultas sobre:

```text
sales
sale_items
products
promotions
product_price_history
```

Conceptualmente:

```text
                 PostgreSQL
                     │
       ┌─────────────┼──────────────┐
       ▼             ▼              ▼
     Sales        Products      Promotions
       │             │              │
       └─────────────┼──────────────┘
                     ▼
              Dataset Builder
                     │
                     ▼
                ML Dataset
```

Esto evita duplicar permanentemente información derivada.

---

# 32. Feature Dataset

El dataset final utilizado por Machine Learning puede tener una estructura como:

```text
date
product_id
category_id
quantity
price
promotion_applied
discount_percentage
day_of_week
month
day_of_month
is_weekend
is_holiday
sales_previous_day
sales_last_7_days
sales_avg_7_days
sales_avg_30_days
```

Target:

```text
quantity
```

---

# 33. Historial y reproducibilidad

Para que un modelo pueda reproducirse, se debe conocer:

```text
Modelo
  │
  ├── Dataset version
  ├── Algorithm
  ├── Training date
  ├── Metrics
  └── Model artifact
```

Por ejemplo:

```text
Model v8
---------------------
Algorithm: RandomForest
Dataset: v4
MAE: 3.21
RMSE: 4.87
R2: 0.84
Training: 2026-10-01
```

---

# 34. Data Retention

Inicialmente no se eliminarán automáticamente:

* Ventas.
* SaleItems.
* Predicciones.
* Versiones de modelos.
* Métricas.

Esto permitirá utilizar información histórica para Machine Learning.

Posteriormente se podrá establecer una estrategia de archivado si el volumen aumenta.

---

# 35. Seed Data

La aplicación deberá tener datos iniciales para desarrollo.

### Roles

```text
Admin
Employee
```

### Categorías

```text
Pasteles
Cheesecakes
Cupcakes
Galletas
Postres
```

### Productos

Ejemplos:

```text
Pastel de Chocolate
Pastel de Fresa
Pastel de Vainilla
Tres Leches
Red Velvet
Cheesecake de Fresa
```

Los datos de producción no deben depender de seed data.

---

# 36. Datos sintéticos para Machine Learning

Durante las primeras etapas se utilizará un generador de datos sintéticos.

El generador debe producir:

* Ventas.
* Productos.
* Variación temporal.
* Promociones.
* Estacionalidad.
* Variación aleatoria.

Ejemplo conceptual:

```text
Fin de semana
      ↓
Mayor demanda

Promoción
      ↓
Incremento de demanda

Temporada especial
      ↓
Cambio de demanda

Ruido aleatorio
      ↓
Variación natural
```

El objetivo es generar un dataset suficientemente realista para experimentar con ML.

---

# 37. No almacenar datos derivados innecesariamente

Información como:

```text
ventas de los últimos 7 días
promedio de ventas de los últimos 30 días
```

no necesita almacenarse permanentemente en una tabla de negocio durante el MVP.

Puede calcularse durante el proceso de feature engineering.

Esto evita inconsistencias.

---

# 38. Futuras extensiones de base de datos

La arquitectura permite agregar posteriormente:

```text
Ingredients
Recipes
Inventory
Suppliers
PurchaseOrders
Customers
CustomerOrders
Branches
Employees
Events
WeatherData
```

Por ejemplo:

```text
Product
   │
   ▼
Recipe
   │
   ├── Flour
   ├── Sugar
   ├── Eggs
   └── Chocolate
```
