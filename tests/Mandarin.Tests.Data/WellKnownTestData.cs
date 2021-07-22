using System;
using System.IO;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Newtonsoft.Json;
using Square.Models;

namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
        private static readonly JsonSerializer Serializer = new();

        public static T DeserializeFromFile<T>(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            using var jsonReader = new JsonTextReader(reader);
            return WellKnownTestData.Serializer.Deserialize<T>(jsonReader);
        }

        public static class Commissions
        {
            public const string FixedCommissions = "TestData/Commissions/FixedCommissions.json";
            public const string RecordOfSalesTLM = "TestData/Commissions/RecordOfSales.TLM.json";
        }

        public static class FramePrices
        {
            public static readonly FramePrice Clementine = new()
            {
                ProductCode = Products.ClementineFramed.ProductCode,
                Amount = 35.00M,
                CreatedAt = new DateTime(2021, 06, 10),
            };
        }

        public static class Products
        {
            public static readonly Product Mandarin = new()
            {
                ProductId = new ProductId("SquareId"),
                ProductCode = new ProductCode("TLM-001"),
                ProductName = new ProductName("Mandarin"),
                Description = "It's a Mandarin!",
                UnitPrice = 45.00M,
            };

            public static readonly Product TheTrickster = new()
            {
                ProductId = new ProductId("CatalogId"),
                ProductCode = new ProductCode("HC20W-003"),
                ProductName = new ProductName("The Trickster"),
                Description = "The Trickster.",
                UnitPrice = 11.00m,
            };

            public static readonly Product ClementineFramed = new()
            {
                ProductId = new ProductId("BTWEJWZCPE4XAKZRBJW53DYE"),
                ProductCode = new ProductCode("KT20-001F"),
                ProductName = new ProductName("Clementine (Framed) (Regular)"),
                Description = "vel augue vestibulum ante ipsum primis in",
                UnitPrice = 95.00M,
            };

            public static readonly Product GiftCard = new()
            {
                ProductId = new ProductId("TLM-GC"),
                ProductCode = new ProductCode("TLM-GC"),
                ProductName = new ProductName("eGift Card"),
                Description = "eGift Card",
                UnitPrice = null,
            };
        }

        public static class Stockists
        {
            public static readonly Stockist KelbyTynan = new()
            {
                StockistId = new StockistId(1),
                StockistCode = new StockistCode("KT20"),
                StatusCode = StatusMode.Inactive,
                Details = new StockistDetail
                {
                    StockistId = new StockistId(1),
                    FirstName = "Kelby",
                    LastName = "Tynan",
                    DisplayName = "Kelby Tynan",
                    TwitterHandle = "jharrowing0",
                    InstagramHandle = "jharrowing0",
                    FacebookHandle = null,
                    WebsiteUrl = "https://hhs.gov/velit.png",
                    TumblrHandle = null,
                    EmailAddress = "ccareless0@homestead.com",
                },
                Commission = new Commission
                {
                    CommissionId = new CommissionId(1),
                    StockistId = new StockistId(1),
                    StartDate = new DateTime(2019, 08, 23),
                    EndDate = new DateTime(2019, 11, 23),
                    Rate = 10,
                    InsertedAt = new DateTime(2019, 08, 23, 17, 36, 24, DateTimeKind.Utc),
                },
            };

            public static readonly Stockist OthilieMapples = new()
            {
                StockistId = new StockistId(4),
                StockistCode = new StockistCode("OM19"),
                StatusCode = StatusMode.ActiveHidden,
                Details = new StockistDetail
                {
                    StockistId = new StockistId(4),
                    FirstName = "Othilie",
                    LastName = "Mapples",
                    DisplayName = "Othilie Mapples",
                    TwitterHandle = "ropfer3",
                    InstagramHandle = "ropfer3",
                    FacebookHandle = null,
                    WebsiteUrl = "http://mtv.com/non/mauris/morbi.jsp",
                    TumblrHandle = null,
                    EmailAddress = "jgunny3@unicef.org",
                },
                Commission = new Commission
                {
                    CommissionId = new CommissionId(4),
                    StockistId = new StockistId(4),
                    StartDate = new DateTime(2019, 01, 16),
                    EndDate = new DateTime(2019, 04, 16),
                    Rate = 40,
                    InsertedAt = new DateTime(2019, 01, 16, 17, 36, 24, DateTimeKind.Utc),
                },
            };

            public static readonly Stockist ArlueneWoodes = new()
            {
                StockistCode = new StockistCode("AW20"),
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    TwitterHandle = "fokennavain0",
                    FirstName = "Arluene",
                    LastName = "Woodes",
                    DisplayName = "Arluene Woodes",
                    InstagramHandle = null,
                    FacebookHandle = null,
                    WebsiteUrl = "https://amazon.co.jp/integer/aliquet/massa/id/lobortis/convallis.png",
                    TumblrHandle = null,
                    EmailAddress = "emorrieson0@amazon.de",
                },
                Commission = new Commission
                {
                    StartDate = new DateTime(2019, 11, 28),
                    EndDate = new DateTime(2020, 06, 09),
                    Rate = 20,
                },
            };

            public static readonly Stockist TheLittleMandarin = new()
            {
                StockistCode = new StockistCode("TLM"),
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    TwitterHandle = "TLM",
                    FirstName = "Little",
                    LastName = "Mandarin",
                    DisplayName = "The Little Mandarin Team",
                    InstagramHandle = "TLM",
                },
                Commission = new Commission
                {
                    StartDate = new DateTime(2019, 11, 28),
                    EndDate = new DateTime(2021, 07, 09),
                    Rate = 10,
                },
            };
        }

        public static class Square
        {
            public class CatalogApi
            {
                public class SearchCatalogObjects
                {
                    public static SearchCatalogObjectsResponse Page1 => WellKnownTestData.DeserializeFromFile<SearchCatalogObjectsResponse>("TestData/Square/CatalogApi/SearchCatalogObjects.1.json");

                    public static SearchCatalogObjectsResponse Page2 => WellKnownTestData.DeserializeFromFile<SearchCatalogObjectsResponse>("TestData/Square/CatalogApi/SearchCatalogObjects.2.json");
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
