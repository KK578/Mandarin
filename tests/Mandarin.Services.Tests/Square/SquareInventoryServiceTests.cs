using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Models.Inventory;
using Mandarin.Services.Square;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
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
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private Mock<ISquareClient> squareClient;

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
            var catalogObjects = await WhenListingInventory();
            Assert.That(catalogObjects.Count, Is.EqualTo(2));
            Assert.That(catalogObjects[0].ProductCode, Is.EqualTo("ID-1"));
            Assert.That(catalogObjects[1].ProductCode, Is.EqualTo("ID-2"));
        }

        private void GivenSquareClientCatalogApiReturnsData()
        {
            this.squareClient = new Mock<ISquareClient>();
            this.squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(null, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => SquareInventoryServiceTests.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage1));
            this.squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => SquareInventoryServiceTests.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2));
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

        private Task<IList<Product>> WhenListingInventory(CancellationToken ct = default)
        {
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, this.squareClient.Object);
            return subject.GetInventory().ToList().ToTask(ct);
        }

        private static T DeserializeFromFile<T>(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            using var jsonReader = new JsonTextReader(reader);
            return SquareInventoryServiceTests.Serializer.Deserialize<T>(jsonReader);
        }
    }
}
