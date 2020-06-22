using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Configuration;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;
using Mandarin.Services.Square;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Square;
using Square.Models;

namespace Mandarin.Services.Tests.Square
{
    [TestFixture]
    public class SquareInventoryServiceTests
    {
        private Mock<ISquareClient> squareClient;

        [Test]
        public async Task GetFixedCommissionAmounts_GivenFileExists_ShouldContainAllObjects()
        {
            this.squareClient = new Mock<ISquareClient>();
            var data = TestData.Create<List<FixedCommissionAmount>>();
            var filename = await this.GivenTemporaryFileExists(data);
            var actual = await this.WhenListingFixedCommissionAmounts(filename);

            actual.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetInventory_WhenRequestIsCancelled_ShouldThrowException()
        {
            var waitHandle = this.GivenSquareClientCatalogApiWaitsToContinue();
            var cts = new CancellationTokenSource();
            var task = this.WhenListingInventory(cts.Token);
            cts.Cancel();
            waitHandle.Set();
            Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Test]
        public async Task GetInventory_WhenServiceListsMultiplePages_ShouldContainAllObjects()
        {
            this.GivenSquareClientCatalogApiReturnsData();
            var catalogObjects = await this.WhenListingInventory();
            Assert.That(catalogObjects.Count, Is.EqualTo(2));
            Assert.That(catalogObjects[0].ProductCode, Is.EqualTo("ID-1"));
            Assert.That(catalogObjects[1].ProductCode, Is.EqualTo("ID-2"));
        }

        private async Task<string> GivenTemporaryFileExists(List<FixedCommissionAmount> data)
        {
            var filename = Path.GetTempFileName();
            var json = JsonConvert.SerializeObject(data);
            await File.WriteAllTextAsync(filename, json);
            return filename;
        }

        private void GivenSquareClientCatalogApiReturnsData()
        {
            this.squareClient = new Mock<ISquareClient>();
            this.squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(null, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage1));
            this.squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2));
        }

        private ManualResetEvent GivenSquareClientCatalogApiWaitsToContinue()
        {
            var waitHandle = new ManualResetEvent(false);
            this.squareClient = new Mock<ISquareClient>();
            this.squareClient
                .Setup(x => x.CatalogApi.ListCatalogAsync(null, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    waitHandle.WaitOne();
                    return new ListCatalogResponse();
                }));

            return waitHandle;
        }

        private Task<IList<FixedCommissionAmount>> WhenListingFixedCommissionAmounts(string filename)
        {
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename, };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, this.squareClient.Object, Options.Create(configuration));
            return subject.GetFixedCommissionAmounts().ToList().ToTask();
        }

        private Task<IList<Product>> WhenListingInventory(CancellationToken ct = default)
        {
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, this.squareClient.Object, Options.Create(new MandarinConfiguration()));
            return subject.GetInventory().ToList().ToTask(ct);
        }
    }
}
