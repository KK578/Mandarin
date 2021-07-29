-- BASHI-123: Persist Square data to database
CREATE TABLE IF NOT EXISTS inventory.product
(
    product_id     VARCHAR(32)   PRIMARY KEY,
    product_code   VARCHAR(12)   NOT NULL,
    product_name   VARCHAR(100)  NOT NULL,
    description    VARCHAR(1000),
    unit_price     NUMERIC(6, 2),
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
    VALUES ($1, $2, $3, $4, $5, $6)
    ON CONFLICT (product_id) DO UPDATE SET (product_code, product_name, description, unit_price, last_updated) = ($2, $3, $4, $5, $6);
END
$$;

CALL inventory.sp_product_upsert('TLM-GC'::varchar(32), 'TLM-GC'::varchar(12), 'eGift Card'::varchar(100), 'eGift Card'::varchar(1000), NULL::NUMERIC(6,2), CURRENT_TIMESTAMP(3)::timestamp);
CALL inventory.sp_product_upsert('TLM-DELIVERY'::varchar(32), 'TLM-DELIVERY'::varchar(12), 'Shipping Fees'::varchar(100), 'Delivery costs charged to customers'::varchar(1000), 0.01::NUMERIC(6,2), CURRENT_TIMESTAMP(3)::timestamp);
CALL inventory.sp_product_upsert('TLM-FRAMING'::varchar(32), 'TLM-FRAMING'::varchar(12), 'Commission for Frame'::varchar(100), 'Commission for Frame'::varchar(1000), 0.01::NUMERIC(6,2), CURRENT_TIMESTAMP(3)::timestamp);
CALL inventory.sp_product_upsert('TLM-TIP'::varchar(32), 'TLM-TIP'::varchar(12), 'Customer Tip'::varchar(100), 'Customer Tip'::varchar(1000), 0.01::NUMERIC(6,2), CURRENT_TIMESTAMP(3)::timestamp);
