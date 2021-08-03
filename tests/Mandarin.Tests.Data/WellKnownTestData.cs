using System;
using System.Collections.Generic;
using System.IO;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Newtonsoft.Json;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

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
                ProductId = ProductId.Of("SquareId"),
                ProductCode = ProductCode.Of("TLM-001"),
                ProductName = ProductName.Of("Mandarin"),
                Description = "It's a Mandarin!",
                UnitPrice = 45.00M,
            };

            public static readonly Product TheTrickster = new()
            {
                ProductId = ProductId.Of("CatalogId"),
                ProductCode = ProductCode.Of("HC20W-003"),
                ProductName = ProductName.Of("The Trickster"),
                Description = "The Trickster.",
                UnitPrice = 11.00m,
            };

            public static readonly Product Clementine = new()
            {
                ProductId = new ProductId("BQGTKYVIFNM6MPB57Y5QEBYN"),
                ProductCode = new ProductCode("KT20-001"),
                ProductName = new ProductName("Clementine (Regular)"),
                Description = "vel augue vestibulum ante ipsum primis in",
                UnitPrice = 45.00M,
                LastUpdated = new DateTime(2021, 01, 31, 22, 51, 49, 569),
            };

            public static readonly Product ClementineFramed = new()
            {
                ProductId = ProductId.Of("BTWEJWZCPE4XAKZRBJW53DYE"),
                ProductCode = ProductCode.Of("KT20-001F"),
                ProductName = ProductName.Of("Clementine (Framed) (Regular)"),
                Description = "vel augue vestibulum ante ipsum primis in",
                UnitPrice = 95.00M,
                LastUpdated = new DateTime(2021, 01, 31, 22, 48, 44, 608),
            };

            public static readonly Product GiftCard = new()
            {
                ProductId = ProductId.Of("TLM-GC"),
                ProductCode = ProductCode.Of("TLM-GC"),
                ProductName = ProductName.Of("eGift Card"),
                Description = "eGift Card",
                UnitPrice = null,
            };
        }

        public static class Stockists
        {
            public static readonly Stockist KelbyTynan = new()
            {
                StockistId = StockistId.Of(1),
                StockistCode = StockistCode.Of("KT20"),
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    StockistId = StockistId.Of(1),
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
                    CommissionId = CommissionId.Of(1),
                    StockistId = StockistId.Of(1),
                    StartDate = new DateTime(2019, 08, 23),
                    EndDate = new DateTime(2019, 11, 23),
                    Rate = 10,
                    InsertedAt = new DateTime(2019, 08, 23, 17, 36, 24, DateTimeKind.Utc),
                },
            };

            public static readonly Stockist OthilieMapples = new()
            {
                StockistId = StockistId.Of(4),
                StockistCode = StockistCode.Of("OM19"),
                StatusCode = StatusMode.ActiveHidden,
                Details = new StockistDetail
                {
                    StockistId = StockistId.Of(4),
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
                    CommissionId = CommissionId.Of(4),
                    StockistId = StockistId.Of(4),
                    StartDate = new DateTime(2019, 01, 16),
                    EndDate = new DateTime(2019, 04, 16),
                    Rate = 40,
                    InsertedAt = new DateTime(2019, 01, 16, 17, 36, 24, DateTimeKind.Utc),
                },
            };

            public static readonly Stockist ArlueneWoodes = new()
            {
                StockistCode = StockistCode.Of("AW20"),
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
                StockistCode = StockistCode.Of("TLM"),
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

        public static class Transactions
        {
            public static readonly Transaction Transaction1 = new()
            {
                ExternalTransactionId = new ExternalTransactionId("sNVseFoHwzywEiVV69mNfK5eV"),
                Timestamp = new DateTime(2021, 07, 14, 12, 54, 06),
                TotalAmount = 45.00M,
                Subtransactions = new List<Subtransaction>
                {
                    new()
                    {
                        Product = Products.Clementine,
                        Quantity = 1,
                        Subtotal = 45.00M,
                    },
                }.AsReadOnly(),
            };
        }
    }
}
