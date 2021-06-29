-- BASHI-118: Allow Frame Prices to Differ by Transaction Date
ALTER TABLE inventory.frame_price
    ADD COLUMN created_at TIMESTAMP(3) DEFAULT CURRENT_TIMESTAMP(3),
    ADD COLUMN active_until TIMESTAMP(3) NULL;

-- noinspection SqlWithoutWhere
UPDATE inventory.frame_price
    SET created_at = '2019-06-01';

ALTER TABLE inventory.frame_price
    ALTER COLUMN created_at SET NOT NULL,
    DROP CONSTRAINT fixed_commission_amount_pkey,
    ADD PRIMARY KEY (product_code, created_at);
