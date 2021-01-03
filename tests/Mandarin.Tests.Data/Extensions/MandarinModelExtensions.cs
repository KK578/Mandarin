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
                FirstName = model.FirstName,
                LastName = model.LastName,
                Details = model.Details,
                Commissions = model.Commissions,
                StatusCode = statusMode,
            };
        }

        public static Stockist WithTenPercentCommission(this Stockist model)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                FirstName = model.FirstName,
                LastName = model.LastName,
                Details = model.Details,
                Commissions = new List<Commission>
                {
                    new()
                    {
                        StartDate = model.Commissions.First().StartDate,
                        EndDate = model.Commissions.First().EndDate,
                        RateGroup = new CommissionRateGroup
                        {
                            Rate = 10,
                        },
                    },
                },
                StatusCode = model.StatusCode,
            };
        }

        public static Stockist WithTlmStockistCode(this Stockist model)
        {
            return new()
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                FirstName = model.FirstName,
                LastName = model.LastName,
                Details = model.Details,
                Commissions = model.Commissions,
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
