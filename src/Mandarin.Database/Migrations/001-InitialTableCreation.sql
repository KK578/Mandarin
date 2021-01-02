CREATE SCHEMA IF NOT EXISTS inventory;
CREATE SCHEMA IF NOT EXISTS billing;

CREATE TABLE IF NOT EXISTS billing.commission_rate_group
(
    group_id SERIAL PRIMARY KEY,
    rate     INT -- TODO: NOT NULL CHECK (rate BETWEEN 0 AND 100)
);

CREATE TABLE IF NOT EXISTS inventory.stockist
(
    stockist_id     SERIAL PRIMARY KEY,
    stockist_code   VARCHAR(6)  NOT NULL UNIQUE,
    stockist_status VARCHAR(25) NOT NULL, -- TODO: Consider ENUM?
    first_name      VARCHAR(100),         -- TODO: Move to stockist_detail
    last_name       VARCHAR(100)          -- TODO: Move to stockist_detail
);

CREATE TABLE IF NOT EXISTS inventory.stockist_detail
(
    stockist_id         INT PRIMARY KEY REFERENCES inventory.stockist (stockist_id),
    twitter_handle      varchar(30),
    instagram_handle    varchar(30),
    facebook_handle     varchar(30),
    website_url         varchar(150),
    image_url           varchar(150),
    tumblr_handle       varchar(30),
    email_address       varchar(100),
    description         varchar(500),
    full_display_name   varchar(250),
    short_display_name  varchar(250),
    thumbnail_image_url varchar(150)
);

CREATE TABLE IF NOT EXISTS billing.commission
(
    commission_id SERIAL PRIMARY KEY,
    stockist_id   INT REFERENCES inventory.stockist (stockist_id),         -- TODO: NOT NULL
    start_date    DATE NOT NULL DEFAULT CURRENT_DATE,
    end_date      DATE NOT NULL,
    rate_group    INT REFERENCES billing.commission_rate_group (group_id), -- TODO: NOT NULL
    inserted_at   TIMESTAMP     DEFAULT CURRENT_TIMESTAMP
);
