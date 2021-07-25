-- BASHI-129: Persist Square transaction details to database
CREATE TABLE IF NOT EXISTS billing.transaction
(
    transaction_id VARCHAR(32) PRIMARY KEY,
    total_amount   NUMERIC(6, 2) NOT NULL,
    timestamp      TIMESTAMP(3)  NOT NULL
);

CREATE TABLE IF NOT EXISTS billing.subtransaction
(
    subtransaction_id SERIAL PRIMARY KEY,
    transaction_id    VARCHAR(32)   NOT NULL REFERENCES billing.transaction (transaction_id),
    product_id        VARCHAR(32)   NOT NULL REFERENCES inventory.product (product_id),
    quantity          INT           NOT NULL,
    subtotal          NUMERIC(6, 2) NOT NULL
);

CREATE TYPE billing.TVP_SUBTRANSACTION AS
(
    product_id VARCHAR(32),
    quantity   INT,
    subtotal   NUMERIC(6, 2)
);

CREATE OR REPLACE PROCEDURE billing.sp_transaction_upsert(
    _transaction_id VARCHAR(32),
    _total_amount NUMERIC(6, 2),
    _timestamp TIMESTAMP(3),
    _subtransactions billing.TVP_SUBTRANSACTION[])
    LANGUAGE plpgsql
AS
$$
DECLARE
    _subtransaction billing.TVP_SUBTRANSACTION;
BEGIN
    INSERT INTO billing.transaction (transaction_id, total_amount, timestamp)
    VALUES ($1, $2, $3)
    ON CONFLICT (transaction_id) DO UPDATE SET (total_amount, timestamp) = ($2, $3);

    DELETE FROM billing.subtransaction WHERE transaction_id = $1;

    FOREACH _subtransaction IN ARRAY $4
    LOOP
        INSERT INTO billing.subtransaction (transaction_id, product_id, quantity, subtotal)
        SELECT $1, _subtransaction.product_id, _subtransaction.quantity, _subtransaction.subtotal;
    END LOOP;
END
$$;
