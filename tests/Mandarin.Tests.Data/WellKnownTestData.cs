using System.IO;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public static async Task SeedDatabaseAsync(MandarinDbContext mandarinDbContext)
        {
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
            public static readonly Stockist InactiveArtist = new Stockist
            {
                StockistCode = "IA1",
                FirstName = "Inactive",
                LastName = "Artist",
                StatusCode = StatusMode.Inactive,
                Details = new StockistDetail
                {
                    ShortDisplayName = nameof(Stockists.InactiveArtist),
                    FullDisplayName = nameof(Stockists.InactiveArtist),
                    Description = "Artist's Description.",
                    BannerImageUrl = "https://localhost/static/images/artist1.jpg",
                },
            };

            public static readonly Stockist MinimalArtist = new Stockist
            {
                StockistCode = "MA1",
                FirstName = "Minimal",
                LastName = "Artist",
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    ShortDisplayName = nameof(Stockists.MinimalArtist),
                    FullDisplayName = nameof(Stockists.MinimalArtist),
                    Description = "Artist's Description.",
                    BannerImageUrl = "https://localhost/static/images/artist1.jpg",
                },
            };

            public static readonly Stockist HiddenArtist = new Stockist
            {
                StockistCode = "HA1",
                FirstName = "Hidden",
                LastName = "Artist",
                StatusCode = StatusMode.ActiveHidden,
                Details = new StockistDetail
                {
                    ShortDisplayName = nameof(Stockists.HiddenArtist),
                    FullDisplayName = nameof(Stockists.HiddenArtist),
                    Description = "Artist's Description.",
                    BannerImageUrl = "https://localhost/static/images/artist1.jpg",
                },
            };

            public static readonly Stockist TheLittleMandarin = new Stockist
            {
                StockistCode = "TLM",
                FirstName = "Little",
                LastName = "Mandarin",
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    ShortDisplayName = "The Little Mandarin",
                    FullDisplayName = "The Little Mandarin",
                    Description = "The Little Mandarin in-house art team!",
                    InstagramHandle = "thelittlemandarin_e17",
                    WebsiteUrl = "https://thelittlemandarin.co.uk/",
                    BannerImageUrl = "https://thelittlemandarin.co.uk/static/images/artists/TLM.jpeg",
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
