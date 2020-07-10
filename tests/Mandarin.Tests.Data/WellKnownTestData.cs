using System.IO;
using System.Threading.Tasks;
using Mandarin.Models.Artists;
using Mandarin.Models.Common;
using Mandarin.Services.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public static async Task SeedDatabaseAsync(MandarinDbContext mandarinDbContext)
        {
            var status = await mandarinDbContext.Status.FirstOrDefaultAsync(x => x.StatusCode == "ACTIVE");
            if (status == null)
            {
                await mandarinDbContext.Status.AddAsync(new Status
                {
                    StatusId = 1,
                    StatusCode = "ACTIVE",
                    Description = "Currently active.",
                });
            }

            var tlmStockist = await mandarinDbContext.Stockist.FirstOrDefaultAsync(x => x.StockistCode == WellKnownTestData.Stockists.TheLittleMandarin.StockistCode);
            if (tlmStockist == null)
            {
                await mandarinDbContext.Stockist.AddAsync(WellKnownTestData.Stockists.TheLittleMandarin);
            }
        }

        public static T DeserializeFromFile<T>(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            using var jsonReader = new JsonTextReader(reader);
            return WellKnownTestData.Serializer.Deserialize<T>(jsonReader);
        }

        public static class Commissions
        {
            public const string ArtistSalesTLM = "TestData/Commissions/ArtistSales.TLM.json";
        }

        public static class Stockists
        {
            public static readonly Stockist FullArtist = new Stockist
            {
                StockistName = "Artist Name",
                StatusCode = "ACTIVE",
                Description = "Artist's Description.",
                Details = new StockistDetail
                {
                    ImageUrl = "https://localhost/static/images/artist1.jpg",
                    TwitterHandle = "ArtistTwitter",
                    InstagramHandle = "ArtistInstagram",
                    FacebookHandle = "ArtistFacebook",
                    TumblrHandle = "ArtistTumblr",
                    WebsiteUrl = "https://localhost/artist/website",
                },
            };

            public static readonly Stockist InactiveArtist = new Stockist
            {
                StockistName = "Artist Name",
                StatusCode = "INACTIVE",
                Description = "Artist's Description.",
                Details = new StockistDetail
                {
                    ImageUrl = "https://localhost/static/images/artist1.jpg",
                },
            };

            public static readonly Stockist MinimalArtist = new Stockist
            {
                StockistName = "Artist Name",
                StatusCode = "ACTIVE",
                Description = "Artist's Description.",
                Details = new StockistDetail
                {
                    ImageUrl = "https://localhost/static/images/artist1.jpg",
                },
            };

            public static readonly Stockist TheLittleMandarin = new Stockist
            {
                StockistCode = "TLM",
                StockistName = "The Little Mandarin",
                StatusCode = "ACTIVE",
                Description = "The Little Mandarin in-house art team!",
                Details = new StockistDetail
                {
                    InstagramHandle = "thelittlemandarin_e17",
                    WebsiteUrl = "https://thelittlemandarin.co.uk/",
                    ImageUrl = "https://thelittlemandarin.co.uk/static/images/artists/TLM.jpeg",
                },
            };
        }

        public static class Square
        {
            public class CatalogApi
            {
                public class ListCatalog
                {
                    public const string ItemsOnlyPage1 = "TestData/Square/CatalogApi/ListCatalog/ListCatalog.ITEM.1.json";
                    public const string ItemsOnlyPage2 = "TestData/Square/CatalogApi/ListCatalog/ListCatalog.ITEM.2.json";
                }
            }

            public class OrdersApi
            {
                public class SearchOrders
                {
                    public const string SearchOrdersPage1 = "TestData/Square/OrdersApi/SearchOrders/SearchOrders.1.json";
                    public const string SearchOrdersPage2 = "TestData/Square/OrdersApi/SearchOrders/SearchOrders.2.json";
                }
            }
        }
    }
}
