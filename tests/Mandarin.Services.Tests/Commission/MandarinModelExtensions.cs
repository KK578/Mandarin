using System.Linq;
using Mandarin.Models.Artists;
using Mandarin.Models.Inventory;
using Mandarin.Models.Transactions;

namespace Mandarin.Services.Tests.Commission
{
    internal static class MandarinModelExtensions
    {
        public static ArtistDetailsModel WithTenPercentCommision(this ArtistDetailsModel model)
        {
            return new ArtistDetailsModel(model.StockistCode,
                                          model.Name,
                                          model.Description,
                                          0.10m,
                                          model.EmailAddress,
                                          model.ImageUrl,
                                          model.TwitterUrl,
                                          model.InstagramUrl,
                                          model.FacebookUrl,
                                          model.TumblrUrl,
                                          model.WebsiteUrl);
        }

        public static ArtistDetailsModel WithTlmStockistCode(this ArtistDetailsModel model)
        {
            return new ArtistDetailsModel("TLM",
                                          model.Name,
                                          model.Description,
                                          model.Rate,
                                          model.EmailAddress,
                                          model.ImageUrl,
                                          model.TwitterUrl,
                                          model.InstagramUrl,
                                          model.FacebookUrl,
                                          model.TumblrUrl,
                                          model.WebsiteUrl);
        }

        public static Transaction WithTlmProducts(this Transaction transaction)
        {
            return new Transaction(transaction.SquareId,
                                   transaction.TotalAmount,
                                   transaction.Timestamp,
                                   transaction.InsertedBy,
                                   transaction.Subtransactions.Select(x => MandarinModelExtensions.WithTlmProducts((Subtransaction)x)));
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
