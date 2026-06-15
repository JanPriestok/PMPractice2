CREATE TABLE addresses (
    id INTEGER PRIMARY KEY,
    postal_code VARCHAR(20),
    area VARCHAR(100),
    city VARCHAR(100),
    street VARCHAR(100),
    house VARCHAR(20)
);

CREATE TABLE partner_types (
    id INTEGER PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE partners (
    id INTEGER PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    type_id INTEGER NOT NULL REFERENCES partner_types(id),
    director_last_name VARCHAR(50),
    director_first_name VARCHAR(50),
    director_middle_name VARCHAR(50),
    email VARCHAR(100),
    phone VARCHAR(20),
    legal_address_id INTEGER NOT NULL REFERENCES addresses(id),
    inn VARCHAR(20) UNIQUE NOT NULL,
    rating INTEGER CHECK (rating >= 0 AND rating <= 100)
);

CREATE TABLE partner_rating_history (
    id SERIAL PRIMARY KEY,
    partner_id INTEGER NOT NULL REFERENCES partners(id),
    old_rating INTEGER,
    new_rating INTEGER NOT NULL CHECK (new_rating >= 0 AND new_rating <= 100),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    changed_by VARCHAR(100)
);

CREATE TABLE product_types (
    id INTEGER PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    coefficient DECIMAL(10,2) NOT NULL
);

CREATE TABLE materials (
    id INTEGER PRIMARY KEY,
    defect_rate DECIMAL(5,2) NOT NULL
);

CREATE TABLE products (
    article VARCHAR(20) PRIMARY KEY,
    type_id INTEGER NOT NULL REFERENCES product_types(id),
    material_id INTEGER REFERENCES materials(id),
    name VARCHAR(200) NOT NULL UNIQUE,
    min_partner_price DECIMAL(12,2) NOT NULL
);

CREATE TABLE sale_headers (
    id SERIAL PRIMARY KEY,
    partner_id INTEGER NOT NULL REFERENCES partners(id),
    sale_date DATE NOT NULL
);

CREATE TABLE sale_items (
    id SERIAL PRIMARY KEY,
    sale_id INTEGER NOT NULL REFERENCES sale_headers(id) ON DELETE CASCADE,
    product_article VARCHAR(20) NOT NULL REFERENCES products(article),
    quantity INTEGER NOT NULL CHECK (quantity > 0)
);
