using System.Collections.Generic;
using System.Linq;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;

namespace Mandarin.Tests.Data.Extensions
{
    public static class MandarinModelExtensions
    {
        public static Stockist AsActive(this Stockist model)
        {
            return new Stockist
            {
                StockistId = model.StockistId,
                StockistCode = model.StockistCode,
                StockistName = model.StockistName,
                Description = model.Description,
                Details = model.Details,
                Commissions = model.Commissions,
                Status = new Status
                {
                    StatusCode = "ACTIVE",
                    Description = "Active",
                },
                StatusCode = "ACTIVE",
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
                Status = model.Status,
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
                Status = model.Status,
                StatusCode = model.StatusCode,
            };
        }

        public static Transaction WithTlmProducts(this Transaction transaction)
        {
            return new Transaction(transaction.SquareId,
                                   transaction.TotalAmount,
                                   transaction.Timestamp,
                                   transaction.InsertedBy,
                                   transaction.Subtransactions.Select(x => x.WithTlmProducts()));
        }

        public static Subtransaction WithTlmProducts(this Subtransaction subtransaction)
        {
            return new Subtransaction(subtransaction.Product.WithTlmProductCode(),
                                      subtransaction.Quantity,
                                      subtransaction.Subtotal);
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
