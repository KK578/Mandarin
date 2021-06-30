-- BASHI-118: Allow Frame Prices to Differ by Transaction Date
CREATE TABLE inventory.new_frame_price
(
    frame_price_id SERIAL PRIMARY KEY,
    product_code   VARCHAR(12)   NOT NULL,
    amount         NUMERIC(6, 2) NOT NULL,
    created_at     TIMESTAMP(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    active_until   TIMESTAMP(3)  NULL
);

INSERT INTO inventory.new_frame_price (product_code, amount, created_at)
SELECT f.product_code, f.amount, '2019-06-01'
FROM inventory.frame_price f;

DROP TABLE inventory.frame_price;
ALTER TABLE inventory.new_frame_price
    RENAME TO frame_price;


CREATE OR REPLACE PROCEDURE inventory.sp_frame_price_upsert(
    _product_code TEXT,
    _amount NUMERIC,
    _created_at TIMESTAMP)
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
$$
