using System;
using System.Collections.Generic;
using System.IO;
using Mandarin.Commissions;
using Mandarin.Common;
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
            public const string RecordOfSalesTLM = "TestData/Commissions/RecordOfSales.TLM.json";
        }

        public static class Stockists
        {
            public static readonly Stockist KelbyTynan = new()
            {
                StockistId = 1,
                StockistCode = "KT20",
                FirstName = "Kelby",
                LastName = "Tynan",
                StatusCode = StatusMode.Inactive,
                Details = new StockistDetail
                {
                    StockistId = 1,
                    TwitterHandle = "jharrowing0",
                    InstagramHandle = "jharrowing0",
                    FacebookHandle = null,
                    WebsiteUrl = "https://hhs.gov/velit.png",
                    BannerImageUrl = "http://dummyimage.com/600x200.png/cc0000/ffffff",
                    TumblrHandle = null,
                    EmailAddress = "ccareless0@homestead.com",
                    Description = "integer pede justo lacinia eget tincidunt eget tempus vel pede morbi porttitor lorem id ligula suspendisse ornare consequat lectus in est risus auctor sed tristique",
                    FullDisplayName = "Kelby Tynan",
                    ShortDisplayName = "Kelby Tynan",
                    ThumbnailImageUrl = "http://dummyimage.com/200x200.png/cc0000/ffffff",
                },
                Commissions = new List<Commission>
                {
                    new()
                    {
                        CommissionId = 1,
                        StockistId = 1,
                        StartDate = new DateTime(2019, 08, 23),
                        EndDate = new DateTime(2019, 11, 23),
                        RateGroupId = 1,
                        InsertedAt = new DateTime(2019, 08, 23, 17, 36, 24, DateTimeKind.Utc),
                        RateGroup = new CommissionRateGroup
                        {
                            GroupId = 1,
                            Rate = 10,
                        },
                    },
                },
            };

            public static readonly Stockist OthilieMapples = new()
            {
                StockistId = 4,
                StockistCode = "OM19",
                FirstName = "Othilie",
                LastName = "Mapples",
                StatusCode = StatusMode.ActiveHidden,
                Details = new StockistDetail
                {
                    StockistId = 4,
                    TwitterHandle = "ropfer3",
                    InstagramHandle = "ropfer3",
                    FacebookHandle = null,
                    WebsiteUrl = "http://mtv.com/non/mauris/morbi.jsp",
                    BannerImageUrl = "http://dummyimage.com/600x200.png/cc0000/ffffff",
                    TumblrHandle = null,
                    EmailAddress = "jgunny3@unicef.org",
                    Description = "nibh in lectus pellentesque at nulla suspendisse potenti cras in purus eu magna vulputate luctus cum sociis natoque penatibus et magnis dis parturient montes nascetur ridiculus mus vivamus vestibulum",
                    FullDisplayName = "Othilie Mapples",
                    ShortDisplayName = "Othilie Mapples",
                    ThumbnailImageUrl = "http://dummyimage.com/200x200.png/ff4444/ffffff",
                },
                Commissions = new List<Commission>
                {
                    new()
                    {
                        CommissionId = 4,
                        StockistId = 4,
                        StartDate = new DateTime(2019, 01, 16),
                        EndDate = new DateTime(2019, 04, 16),
                        RateGroupId = 2,
                        InsertedAt = new DateTime(2019, 01, 16, 17, 36, 24, DateTimeKind.Utc),
                        RateGroup = new CommissionRateGroup
                        {
                            GroupId = 2,
                            Rate = 40,
                        },
                    },
                },
            };

            public static readonly Stockist ArlueneWoodes = new()
            {
                StockistCode = "AW20",
                FirstName = "Arluene",
                LastName = "Woodes",
                StatusCode = StatusMode.Active,
                Details = new StockistDetail
                {
                    TwitterHandle = "fokennavain0",
                    InstagramHandle = null,
                    FacebookHandle = null,
                    WebsiteUrl = "https://amazon.co.jp/integer/aliquet/massa/id/lobortis/convallis.png",
                    BannerImageUrl = "http://dummyimage.com/600x200.png/5fa2dd/ffffff",
                    TumblrHandle = null,
                    EmailAddress = "emorrieson0@amazon.de",
                    Description = "elit proin risus praesent lectus vestibulum quam sapien varius ut blandit non interdum in ante vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae duis faucibus",
                    FullDisplayName = "Arluene Woodes",
                    ShortDisplayName = "Arluene Woodes",
                    ThumbnailImageUrl = "http://dummyimage.com/200x200.png/ff4444/ffffff",
                },
                Commissions = new List<Commission>
                {
                    new()
                    {
                        StartDate = new DateTime(2019, 11, 28),
                        EndDate = new DateTime(2020, 06, 09),
                        RateGroupId = 3,
                        RateGroup = new CommissionRateGroup
                        {
                            GroupId = 3,
                            Rate = 20,
                        },
                    },
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
