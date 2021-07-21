﻿using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Mandarin.Database;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Static
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class ProductSynchronizerTests : MandarinIntegrationTestsBase
    {
        private readonly IProductSynchronizer productSynchronizer;
        private readonly IProductRepository productRepository;

        public ProductSynchronizerTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.productSynchronizer = fixture.Services.GetRequiredService<IProductSynchronizer>();
            this.productRepository = fixture.Services.GetRequiredService<IProductRepository>();
        }

        [Fact]
        public async Task ShouldSaveSquareDataIntoDatabase()
        {
            await this.GivenProductTableIsEmptyAsync();
            await this.productSynchronizer.SynchroniseProductsAsync();
            var products = await this.productRepository.GetAllAsync();
            products.Should().HaveCount(4);
        }

        [Fact]
        public async Task ShouldNotUpdateMultipleTimesIfItemsAlreadyExist()
        {
            await this.productSynchronizer.SynchroniseProductsAsync();
            var products = await this.productRepository.GetAllAsync();
            products.Should().HaveCount(4);
        }

        private async Task GivenProductTableIsEmptyAsync()
        {
            var db = this.Fixture.Services.GetRequiredService<MandarinDbContext>();
            await db.GetConnection().ExecuteAsync("TRUNCATE inventory.product");
        }
    }
}
