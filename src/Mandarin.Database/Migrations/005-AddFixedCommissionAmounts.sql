-- BASHI-105: Move Fixed Commission Amounts into Database
CREATE TABLE IF NOT EXISTS inventory.fixed_commission_amount
(
    product_code VARCHAR(12) PRIMARY KEY,
    amount       NUMERIC(6, 2)
);

WITH temp_fixed_commission_amount_json (doc) AS (
    values ('$FixedCommissionAmountJson$'::json)
)
INSERT INTO inventory.fixed_commission_amount (product_code, amount)
SELECT r.*
FROM temp_fixed_commission_amount_json j
  CROSS JOIN LATERAL json_populate_recordset(null::inventory.fixed_commission_amount, doc) as r;
