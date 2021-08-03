-- BASHI-129: Persist Square transaction details to database
CREATE TABLE IF NOT EXISTS billing.external_transaction
(
    external_transaction_id VARCHAR(32)  PRIMARY KEY NOT NULL,
    updated_at              TIMESTAMP(3) NOT NULL,
    created_at              TIMESTAMP(3) NOT NULL,
    raw_data                JSONB        NOT NULL
);

CREATE TABLE IF NOT EXISTS billing.transaction
(
    transaction_id          SERIAL PRIMARY KEY,
    external_transaction_id VARCHAR(32)   UNIQUE NOT NULL REFERENCES billing.external_transaction (external_transaction_id),
    total_amount            NUMERIC(6, 2) NOT NULL,
    timestamp               TIMESTAMP(3)  NOT NULL
);

CREATE TABLE IF NOT EXISTS billing.subtransaction
(
    subtransaction_id SERIAL PRIMARY KEY,
    transaction_id    INT           NOT NULL REFERENCES billing.transaction (transaction_id),
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
    _external_transaction_id VARCHAR(32),
    _total_amount NUMERIC(6, 2),
    _timestamp TIMESTAMP(3),
    _subtransactions billing.TVP_SUBTRANSACTION[])
    LANGUAGE plpgsql
AS
$$
DECLARE
    _transaction_id INT;
    _subtransaction billing.TVP_SUBTRANSACTION;
BEGIN
    INSERT INTO billing.transaction (external_transaction_id, total_amount, timestamp)
    VALUES ($1, $2, $3)
    ON CONFLICT (external_transaction_id) DO UPDATE SET (total_amount, timestamp) = ($2, $3)
    RETURNING transaction_id INTO _transaction_id;

    DELETE FROM billing.subtransaction WHERE transaction_id = _transaction_id;

    FOREACH _subtransaction IN ARRAY $4
    LOOP
        INSERT INTO billing.subtransaction (transaction_id, product_id, quantity, subtotal)
        SELECT _transaction_id, _subtransaction.product_id, _subtransaction.quantity, _subtransaction.subtotal;
    END LOOP;
END
$$;
