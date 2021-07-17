using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions;

namespace Mandarin.Tests.Data.Extensions
{
    public static class MandarinModelExtensions
    {
        public static Stockist WithoutCommission(this Stockist model)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = model.StockistCode,
                Details = model.Details,
                Commission = null,
                StatusCode = model.StatusCode,
            };
        }

        public static Product WithTlmProductCode(this Product product)
        {
            return new(product.SquareId,
                       new ProductCode($"TLM-{product.ProductCode}"),
                       product.ProductName,
                       product.Description,
                       product.UnitPrice);
        }

        public static Product WithUnitPrice(this Product product, decimal unitPrice)
        {
            return new(product.SquareId,
                       new ProductCode($"TLM-{product.ProductCode}"),
                       product.ProductName,
                       product.Description,
                       unitPrice);
        }
    }
}
