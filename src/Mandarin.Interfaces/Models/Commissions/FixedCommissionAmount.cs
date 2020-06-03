namespace Mandarin.Models.Commissions
{
    public class FixedCommissionAmount
    {
        public FixedCommissionAmount(string productCode, decimal amount)
        {
            this.ProductCode = productCode;
            this.Amount = amount;
        }

        public string ProductCode { get; }
        public decimal Amount { get; }
    }
}
