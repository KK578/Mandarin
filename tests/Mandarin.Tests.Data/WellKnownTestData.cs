using System.IO;
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
                public const string FullArtistData = "TestData/Fruity/Stockist/FullArtistData.json";
                public const string InactiveArtistData = "TestData/Fruity/Stockist/InactiveArtistData.json";
                public const string MinimalArtistData = "TestData/Fruity/Stockist/MinimalArtistData.json";
                public const string TheLittleMandarin = "TestData/Fruity/Stockist/TheLittleMandarin.json";
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
