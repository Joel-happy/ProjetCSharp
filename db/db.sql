CREATE DATABASE IF NOT EXISTS ecommerce;

USE ecommerce;

CREATE TABLE IF NOT EXISTS account (
    account_id INTEGER PRIMARY KEY AUTOINCREMENT,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL,
    password VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS address (
    address_id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id INTEGER,
    street VARCHAR(255),
    city VARCHAR(255),
    FOREIGN KEY (account_id) REFERENCES account(account_id)
);

CREATE TABLE IF NOT EXISTS product (
    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(255),
    description VARCHAR(255),
    price DECIMAL(5,2)
);

CREATE TABLE IF NOT EXISTS cart (
    cart_id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id INTEGER,
    account_id INTEGER,
    quantity INTEGER,
    FOREIGN KEY (product_id) REFERENCES product(product_id),
    FOREIGN KEY (account_id) REFERENCES account(account_id)
);

CREATE TABLE IF NOT EXISTS command (
    command_id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id INTEGER,
    order_date TIMESTAMP,
    status VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS invoices (
    invoice_id INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id INTEGER,
    command_id INTEGER,
    invoice_date TIMESTAMP,
    total_amount DECIMAL(10,2)
);