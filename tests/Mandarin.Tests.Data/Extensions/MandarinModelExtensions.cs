using System.Collections.Generic;
using System.Linq;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Inventory;

namespace Mandarin.Tests.Data.Extensions
{
    public static class MandarinModelExtensions
    {
        public static Stockist WithStatus(this Stockist model, StatusMode statusMode = StatusMode.Active)
        {
            return new Stockist
            {
                StockistId = model.StockistId,
                StockistCode = model.StockistCode,
                StockistName = model.StockistName,
                Description = model.Description,
                Details = model.Details,
                Commissions = model.Commissions,
                StatusCode = statusMode,
            };
        }

        public static Stockist WithTenPercentCommission(this Stockist model)
        {
            return new Stockist
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                StockistName = model.StockistName,
                Description = model.Description,
                Details = model.Details,
                Commissions = new List<Commission>
                {
                    new Commission
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
            return new Stockist
            {
                StockistId = model.StockistId,
                StockistCode = "TLM",
                StockistName = model.StockistName,
                Description = model.Description,
                Details = model.Details,
                Commissions = model.Commissions,
                StatusCode = model.StatusCode,
            };
        }

        public static Product WithTlmProductCode(this Product product)
        {
            return new Product(product.SquareId,
                               $"TLM-{product.ProductCode}",
                               product.ProductName,
                               product.Description,
                               product.UnitPrice);
        }

        public static Product WithUnitPrice(this Product product, decimal unitPrice)
        {
            return new Product(product.SquareId,
                               $"TLM-{product.ProductCode}",
                               product.ProductName,
                               product.Description,
                               unitPrice);
        }
    }
}
