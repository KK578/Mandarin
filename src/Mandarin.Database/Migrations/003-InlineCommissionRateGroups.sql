ALTER TABLE billing.commission
ADD COLUMN IF NOT EXISTS rate INT CHECK (rate BETWEEN 0 AND 100);

UPDATE billing.commission c1
SET rate = crg.rate
FROM billing.commission c2
INNER JOIN billing.commission_rate_group crg ON c2.rate_group = crg.group_id
WHERE c1.commission_id = c2.commission_id;

ALTER TABLE billing.commission DROP COLUMN rate_group;
ALTER TABLE billing.commission ALTER COLUMN rate SET NOT NULL;

DROP TABLE billing.commission_rate_group;
