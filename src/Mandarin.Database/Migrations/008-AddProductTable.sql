-- BASHI-123: Persist Square data to database
CREATE TABLE IF NOT EXISTS inventory.product
(
    product_id     VARCHAR(32)   PRIMARY KEY,
    product_code   VARCHAR(12)   UNIQUE NOT NULL,
    product_name   VARCHAR(100)  NOT NULL,
    description    VARCHAR(1000) NOT NULL,
    unit_price     NUMERIC(6, 2) NOT NULL,
    last_updated   TIMESTAMP(3)  NOT NULL
);


CREATE OR REPLACE PROCEDURE inventory.sp_product_upsert(
    _product_id     VARCHAR(32),
    _product_code   VARCHAR(12),
    _product_name   VARCHAR(100),
    _description    VARCHAR(1000),
    _unit_price     NUMERIC(6, 2),
    _last_updated   TIMESTAMP(3))
    LANGUAGE plpgsql
AS
$$
DECLARE
BEGIN
    INSERT INTO inventory.product (product_id, product_code, product_name, description, unit_price, last_updated)
    VALUES ($1, $2, $3, $4, $5, $6);
END
$$
