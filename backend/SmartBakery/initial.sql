CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;
CREATE TABLE categories (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    description text,
    is_active boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_categories PRIMARY KEY (id)
);

CREATE TABLE model_versions (
    id uuid NOT NULL,
    version character varying(50) NOT NULL,
    algorithm character varying(100) NOT NULL,
    dataset_version character varying(100) NOT NULL,
    model_path text NOT NULL,
    status character varying(30) NOT NULL,
    training_date timestamp with time zone NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_model_versions PRIMARY KEY (id)
);

CREATE TABLE promotions (
    id uuid NOT NULL,
    name character varying(150) NOT NULL,
    description text,
    discount_percentage numeric(5,2) NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    is_active boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_promotions PRIMARY KEY (id),
    CONSTRAINT ck_promotions_dates CHECK (end_date >= start_date),
    CONSTRAINT ck_promotions_discount_range CHECK (discount_percentage > 0 AND discount_percentage <= 100)
);

CREATE TABLE roles (
    id uuid NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255),
    CONSTRAINT pk_roles PRIMARY KEY (id)
);

CREATE TABLE products (
    id uuid NOT NULL,
    category_id uuid NOT NULL,
    name character varying(150) NOT NULL,
    description text,
    price numeric(10,2) NOT NULL,
    is_active boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_products PRIMARY KEY (id),
    CONSTRAINT ck_products_price_positive CHECK (price > 0),
    CONSTRAINT fk_products_categories_category_id FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE RESTRICT
);

CREATE TABLE users (
    id uuid NOT NULL,
    username character varying(100) NOT NULL,
    email character varying(255) NOT NULL,
    password_hash text NOT NULL,
    role_id uuid NOT NULL,
    is_active boolean NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_users PRIMARY KEY (id),
    CONSTRAINT fk_users_roles_role_id FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE RESTRICT
);

CREATE TABLE predictions (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    prediction_date date NOT NULL,
    predicted_quantity numeric(10,2) NOT NULL,
    model_version_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_predictions PRIMARY KEY (id),
    CONSTRAINT ck_predictions_quantity_non_negative CHECK (predicted_quantity >= 0),
    CONSTRAINT fk_predictions_model_versions_model_version_id FOREIGN KEY (model_version_id) REFERENCES model_versions (id) ON DELETE RESTRICT,
    CONSTRAINT fk_predictions_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT
);

CREATE TABLE sales (
    id uuid NOT NULL,
    sale_date timestamp with time zone NOT NULL,
    user_id uuid NOT NULL,
    subtotal numeric(12,2) NOT NULL,
    discount_amount numeric(12,2) NOT NULL,
    total_amount numeric(12,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_sales PRIMARY KEY (id),
    CONSTRAINT ck_sales_discount_non_negative CHECK (discount_amount >= 0),
    CONSTRAINT ck_sales_subtotal_non_negative CHECK (subtotal >= 0),
    CONSTRAINT ck_sales_total_consistent CHECK (total_amount = subtotal - discount_amount),
    CONSTRAINT ck_sales_total_non_negative CHECK (total_amount >= 0),
    CONSTRAINT fk_sales_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE TABLE sale_items (
    id uuid NOT NULL,
    sale_id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(10,2) NOT NULL,
    discount_amount numeric(10,2) NOT NULL,
    subtotal numeric(12,2) NOT NULL,
    CONSTRAINT pk_sale_items PRIMARY KEY (id),
    CONSTRAINT ck_sale_items_discount_non_negative CHECK (discount_amount >= 0),
    CONSTRAINT ck_sale_items_quantity_positive CHECK (quantity > 0),
    CONSTRAINT ck_sale_items_subtotal_non_negative CHECK (subtotal >= 0),
    CONSTRAINT ck_sale_items_unit_price_non_negative CHECK (unit_price >= 0),
    CONSTRAINT fk_sale_items_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sale_items_sales_sale_id FOREIGN KEY (sale_id) REFERENCES sales (id) ON DELETE CASCADE
);

INSERT INTO roles (id, description, name)
VALUES ('01920000-0000-7000-8000-000000000001', 'Full access: products, sales, models and users.', 'Admin');
INSERT INTO roles (id, description, name)
VALUES ('01920000-0000-7000-8000-000000000002', 'Registers sales and views products and predictions.', 'Employee');

CREATE UNIQUE INDEX ix_categories_name ON categories (name);

CREATE INDEX ix_model_versions_status ON model_versions (status);

CREATE UNIQUE INDEX ix_model_versions_status1 ON model_versions (status) WHERE status = 'Production';

CREATE INDEX ix_model_versions_training_date ON model_versions (training_date);

CREATE UNIQUE INDEX ix_model_versions_version ON model_versions (version);

CREATE INDEX ix_predictions_model_version_id ON predictions (model_version_id);

CREATE INDEX ix_predictions_prediction_date ON predictions (prediction_date);

CREATE INDEX ix_predictions_product_id ON predictions (product_id);

CREATE INDEX ix_products_category_id ON products (category_id);

CREATE INDEX ix_products_is_active ON products (is_active);

CREATE INDEX ix_products_name ON products (name);

CREATE UNIQUE INDEX ix_roles_name ON roles (name);

CREATE INDEX ix_sale_items_product_id ON sale_items (product_id);

CREATE INDEX ix_sale_items_sale_id ON sale_items (sale_id);

CREATE INDEX ix_sales_sale_date ON sales (sale_date);

CREATE INDEX ix_sales_user_id ON sales (user_id);

CREATE UNIQUE INDEX ix_users_email ON users (email);

CREATE INDEX ix_users_role_id ON users (role_id);

CREATE UNIQUE INDEX ix_users_username ON users (username);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20261007162620_InitialCreate', '10.0.12');

COMMIT;

