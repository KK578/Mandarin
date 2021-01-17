-- BASHI-102: Remove Public Application Information from Stockists
ALTER TABLE inventory.stockist_detail ADD COLUMN IF NOT EXISTS first_name VARCHAR(100);
ALTER TABLE inventory.stockist_detail ADD COLUMN IF NOT EXISTS last_name VARCHAR(100);

UPDATE inventory.stockist_detail sd1
SET first_name = s.first_name,
    last_name = s.last_name
FROM inventory.stockist s
INNER JOIN inventory.stockist_detail sd2 ON sd2.stockist_id = s.stockist_id
WHERE sd1.stockist_id = sd2.stockist_id;

ALTER TABLE inventory.stockist_detail DROP COLUMN full_display_name;
ALTER TABLE inventory.stockist_detail DROP COLUMN image_url;
ALTER TABLE inventory.stockist_detail DROP COLUMN thumbnail_image_url;
ALTER TABLE inventory.stockist_detail DROP COLUMN description;
ALTER TABLE inventory.stockist DROP COLUMN first_name;
ALTER TABLE inventory.stockist DROP COLUMN last_name;
