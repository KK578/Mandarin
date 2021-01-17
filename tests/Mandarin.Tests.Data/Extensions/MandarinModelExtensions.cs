using System.Collections.Generic;
using System.Linq;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Inventory;
using Mandarin.Stockists;

namespace Mandarin.Tests.Data.Extensions
{
    public static class MandarinModelExtensions
    {
        public static Stockist WithStatus(this Stockist model, StatusMode statusMode)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = model.StockistCode,
                Details = model.Details,
                Commission = model.Commission,
                StatusCode = statusMode,
            };
        }

        public static Stockist WithTenPercentCommission(this Stockist model)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                Details = model.Details,
                Commission = new Commission
                {
                    StartDate = model.Commission.StartDate,
                    EndDate = model.Commission.EndDate,
                    Rate = 10,
                },
                StatusCode = model.StatusCode,
            };
        }

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

        public static Stockist WithTlmStockistCode(this Stockist model)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                Details = model.Details,
                Commission = model.Commission,
                StatusCode = model.StatusCode,
            };
        }

        public static Product WithTlmProductCode(this Product product)
        {
            return new(product.SquareId,
                       $"TLM-{product.ProductCode}",
                       product.ProductName,
                       product.Description,
                       product.UnitPrice);
        }

        public static Product WithUnitPrice(this Product product, decimal unitPrice)
        {
            return new(product.SquareId,
                       $"TLM-{product.ProductCode}",
                       product.ProductName,
                       product.Description,
                       unitPrice);
        }
    }
}
