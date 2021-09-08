INSERT INTO billing.external_transaction (external_transaction_id, updated_at, created_at, raw_data) VALUES ('sNVseFoHwzywEiVV69mNfK5eV', '2021-07-14T11:54:06Z', '2021-07-14T11:54:06Z', CAST($$
{
  "id": "sNVseFoHwzywEiVV69mNfK5eV",
  "state": "COMPLETED",
  "closed_at": "2021-07-14T11:54:06Z",
  "created_at": "2021-07-14T11:54:06Z",
  "line_items": [
    {
      "uid": "1E001F97-BB6A-47DD-8585-19D3F0D818EB",
      "name": "KT20-001 - Clementine",
      "quantity": "1",
      "item_type": "ITEM",
      "total_money": { "amount": 4500, "currency": "GBP" },
      "variation_name": "Regular",
      "total_tax_money": { "amount": 0, "currency": "GBP" },
      "base_price_money": { "amount": 4500, "currency": "GBP" },
      "catalog_object_id": "BQGTKYVIFNM6MPB57Y5QEBYN",
      "gross_sales_money": { "amount": 4500, "currency": "GBP" },
      "total_discount_money": { "amount": 0, "currency": "GBP" },
      "variation_total_price_money": { "amount": 4500, "currency": "GBP" }
    }
  ],
  "updated_at": "2021-07-14T11:54:06Z",
  "net_amounts": {
    "tax_money": { "amount": 0, "currency": "GBP" },
    "tip_money": { "amount": 0, "currency": "GBP" },
    "total_money": { "amount": 4500, "currency": "GBP" },
    "discount_money": { "amount": 0, "currency": "GBP" },
    "service_charge_money": { "amount": 0, "currency": "GBP" }
  },
  "total_money": { "amount": 4500, "currency": "GBP" },
  "return_amounts": {
    "tax_money": { "amount": 0, "currency": "GBP" },
    "tip_money": { "amount": 0, "currency": "GBP" },
    "total_money": { "amount": 0, "currency": "GBP" },
    "discount_money": { "amount": 0, "currency": "GBP" },
    "service_charge_money": { "amount": 0, "currency": "GBP" }
  },
  "total_tax_money": { "amount": 0, "currency": "GBP" },
  "total_tip_money": { "amount": 0, "currency": "GBP" },
  "total_discount_money": { "amount": 0, "currency": "GBP" },
  "total_service_charge_money": { "amount": 0, "currency": "GBP" }
}$$ AS JSON));

CALL billing.sp_transaction_upsert('sNVseFoHwzywEiVV69mNfK5eV',
                                   45,
                                   '2021-07-14T11:54:06Z',
                                   '{"(\"BQGTKYVIFNM6MPB57Y5QEBYN\", 1, 45)"}'::billing.tvp_subtransaction[]);
