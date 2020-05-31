using Mandarin.Converters;
using Mandarin.Models.Transactions;
using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    public class Sale
    {
        public Sale(string productCode,
                    string productName,
                    int quantity,
                    decimal unitPrice,
                    decimal subtotal,
                    decimal commission,
                    decimal total)
        {
            this.ProductCode = productCode;
            this.ProductName = productName;
            this.Quantity = quantity;
            this.UnitPrice = unitPrice;
            this.Subtotal = subtotal;
            this.Commission = commission;
            this.Total = total;
        }

        [JsonProperty("productCode")] public string ProductCode { get; }
        [JsonProperty("productName")] public string ProductName { get; }
        [JsonProperty("quantity")] public int Quantity { get; set; }
        [JsonProperty("unitPrice")] [JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal UnitPrice { get; }
        [JsonProperty("subtotal")] [JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal Subtotal { get; }
        [JsonProperty("commission")] [JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal Commission { get; }
        [JsonProperty("total")] [JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal Total { get; }

        public override string ToString()
        {
            return $"{this.ProductCode}: {this.Quantity} * {this.UnitPrice:C} = {this.Subtotal} ({nameof(Sale.Commission)}: {this.Commission:C} => {this.Total:C})";
        }
    }

    public static class SaleMapper
    {
        public static Sale FromTransaction(Subtransaction subtransaction, decimal rate, FixedCommissionAmount fixedCommission)
        {
            var fixedAmount = fixedCommission?.Amount ?? 0;
            var subTotal = subtransaction.Subtotal - fixedAmount * subtransaction.Quantity;
            var commission = subTotal * rate;
            var sale = new Sale(subtransaction.Product.ProductCode,
                                subtransaction.Product.ProductName,
                                subtransaction.Quantity,
                                subtransaction.TransactionUnitPrice - fixedAmount,
                                subTotal,
                                -commission,
                                subTotal - commission);

            return sale;
        }
    }
}
