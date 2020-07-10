using System.IO;
using Mandarin.Models.Artists;
using Newtonsoft.Json;

namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

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

        public static class Fruity
        {
            public static class Stockist
            {
                public const string TheLittleMandarin = "TestData/Fruity/Stockist/TheLittleMandarin.json";

                public static readonly Mandarin.Models.Artists.Stockist FullArtist = new Models.Artists.Stockist
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

                public static readonly Mandarin.Models.Artists.Stockist InactiveArtist = new Models.Artists.Stockist
                {
                    StockistName = "Artist Name",
                    StatusCode = "INACTIVE",
                    Description = "Artist's Description.",
                    Details = new StockistDetail
                    {
                        ImageUrl = "https://localhost/static/images/artist1.jpg",
                    },
                };

                public static readonly Mandarin.Models.Artists.Stockist MinimalArtist = new Models.Artists.Stockist
                {
                    StockistName = "Artist Name",
                    StatusCode = "ACTIVE",
                    Description = "Artist's Description.",
                    Details = new StockistDetail
                    {
                        ImageUrl = "https://localhost/static/images/artist1.jpg",
                    },
                };
            }
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
