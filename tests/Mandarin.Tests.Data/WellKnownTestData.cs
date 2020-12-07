﻿using System.IO;
using System.Threading.Tasks;
using Mandarin.Database;
using Mandarin.Models.Artists;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Square.Models;

namespace Mandarin.Tests.Data
{
    public static class WellKnownTestData
    {
        private static readonly JsonSerializer Serializer = new();

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
            public static readonly Stockist InactiveArtist = new()
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

            public static readonly Stockist MinimalArtist = new()
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

            public static readonly Stockist HiddenArtist = new()
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

            public static readonly Stockist TheLittleMandarin = new()
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
