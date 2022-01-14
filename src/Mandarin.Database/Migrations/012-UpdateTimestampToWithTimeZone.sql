-- BASHI-135: Upgrade Npgsql to 6.0.0
SET TimeZone='UTC';

ALTER TABLE billing.commission ALTER COLUMN inserted_at TYPE timestamp WITH TIME ZONE;
ALTER TABLE billing.external_transaction ALTER COLUMN updated_at TYPE timestamp WITH TIME ZONE;
ALTER TABLE billing.external_transaction ALTER COLUMN created_at TYPE timestamp WITH TIME ZONE;
ALTER TABLE billing.transaction ALTER COLUMN timestamp TYPE timestamp WITH TIME ZONE;
ALTER TABLE inventory.frame_price ALTER COLUMN created_at TYPE timestamp WITH TIME ZONE;
ALTER TABLE inventory.frame_price ALTER COLUMN active_until TYPE timestamp WITH TIME ZONE;
ALTER TABLE inventory.product ALTER COLUMN last_updated TYPE timestamp WITH TIME ZONE;

DROP PROCEDURE inventory.sp_frame_price_upsert(TEXT, NUMERIC, TIMESTAMP);
DROP PROCEDURE inventory.sp_product_upsert(VARCHAR, VARCHAR, VARCHAR, VARCHAR, NUMERIC, TIMESTAMP);
DROP PROCEDURE billing.sp_transaction_upsert(VARCHAR, NUMERIC, TIMESTAMP, billing.tvp_subtransaction[]);

CREATE OR REPLACE PROCEDURE inventory.sp_frame_price_upsert(
    _product_code TEXT,
    _amount NUMERIC,
    _created_at TIMESTAMP WITH TIME ZONE)
    LANGUAGE plpgsql
AS
$$
DECLARE
BEGIN
    UPDATE inventory.frame_price
    SET active_until = $3
    WHERE product_code = $1
      AND active_until IS NULL;

    INSERT INTO inventory.frame_price (product_code, amount, created_at)
    VALUES ($1, $2, $3);
END
$$;

CREATE OR REPLACE PROCEDURE inventory.sp_product_upsert(
    _product_id     VARCHAR(32),
    _product_code   VARCHAR(12),
    _product_name   VARCHAR(100),
    _description    VARCHAR(1000),
    _unit_price     NUMERIC(6, 2),
    _last_updated   TIMESTAMP(3) WITH TIME ZONE)
    LANGUAGE plpgsql
AS
$$
DECLARE _stockist_id INT;
BEGIN
    SELECT stockist_id INTO _stockist_id
    FROM inventory.stockist
    WHERE stockist_code = left($2, strpos($2, '-') - 1);

    INSERT INTO inventory.product (product_id, stockist_id, product_code, product_name, description, unit_price, last_updated)
    VALUES ($1, _stockist_id, $2, $3, $4, $5, $6)
    ON CONFLICT (product_id) DO UPDATE SET (stockist_id, product_code, product_name, description, unit_price, last_updated) = (_stockist_id, $2, $3, $4, $5, $6);
END
$$;

CREATE OR REPLACE PROCEDURE billing.sp_transaction_upsert(
    _external_transaction_id VARCHAR(32),
    _total_amount NUMERIC(6, 2),
    _timestamp TIMESTAMP(3) WITH TIME ZONE,
    _subtransactions billing.TVP_SUBTRANSACTION[])
    LANGUAGE plpgsql
AS
$$
DECLARE
    _transaction_id INT;
    _commission_rate INT;
    _subtransaction billing.TVP_SUBTRANSACTION;
BEGIN
    INSERT INTO billing.transaction (external_transaction_id, total_amount, timestamp)
    VALUES ($1, $2, $3)
    ON CONFLICT (external_transaction_id) DO UPDATE SET (total_amount, timestamp) = ($2, $3)
    RETURNING transaction_id INTO _transaction_id;

    DELETE FROM billing.subtransaction WHERE transaction_id = _transaction_id;

    FOREACH _subtransaction IN ARRAY $4
        LOOP
            SELECT c.rate INTO _commission_rate
            FROM inventory.product p
                     INNER JOIN inventory.stockist s ON s.stockist_id = p.stockist_id
                     INNER JOIN billing.commission c ON c.stockist_id = s.stockist_id
            WHERE p.product_id = _subtransaction.product_id
            ORDER BY c.inserted_at DESC LIMIT 1;

            INSERT INTO billing.subtransaction (transaction_id, product_id, quantity, unit_price, commission_rate)
            SELECT _transaction_id, _subtransaction.product_id, _subtransaction.quantity, _subtransaction.unit_price, _commission_rate;
        END LOOP;
END
$$;
