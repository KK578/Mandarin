namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
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
