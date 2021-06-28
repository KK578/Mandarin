-- BASHI-117: Rename Fixed Commissions Amounts to Frame Prices
ALTER TABLE inventory.fixed_commission_amount
    RENAME TO frame_price;
