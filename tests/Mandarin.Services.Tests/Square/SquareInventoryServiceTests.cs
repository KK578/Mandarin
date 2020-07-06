using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Configuration;
using Mandarin.Models.Commissions;
using Mandarin.Services.Square;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Square;
using Square.Models;

namespace Mandarin.Services.Tests.Square
{
    [TestFixture]
    public class SquareInventoryServiceTests
    {
        [Test]
        public async Task AddFixedCommissionAmount_GivenDataExists_ShouldAppendValue()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));

            var additionalData = TestData.Create<FixedCommissionAmount>();
            await subject.AddFixedCommissionAmount(additionalData);

            var result = await subject.GetFixedCommissionAmounts().ToList();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[1].ProductCode, Is.EqualTo(additionalData.ProductCode));
        }

        [Test]
        public async Task UpdateFixedCommissionAmount_GivenDataExists_ShouldAppendValue()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));

            var additionalData = new FixedCommissionAmount(data.ProductCode, 12.34M);
            await subject.UpdateFixedCommissionAmount(additionalData);

            var result = await subject.GetFixedCommissionAmounts().ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ProductCode, Is.EqualTo(additionalData.ProductCode));
            Assert.That(result[0].Amount, Is.EqualTo(additionalData.Amount));
        }

        [Test]
        public async Task DeleteFixedCommissionAmount_GivenDataExists_ShouldAppendValue()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));

            await subject.DeleteFixedCommissionAmount(data.ProductCode);

            var result = await subject.GetFixedCommissionAmounts().ToList();
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetFixedCommissionAmounts_GivenEmptyFileName_ShouldReturnEmpty()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = string.Empty };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public async Task GetFixedCommissionAmounts_GivenNonExistingFile_ShouldReturnEmpty()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = "NonExistantFile.json" };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public async Task GetFixedCommissionAmounts_GivenFileExists_ShouldContainAllObjects()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();

            Assert.That(actual, Has.Exactly(1).Items);
            actual[0].Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetInventory_WhenRequestIsCancelled_ShouldThrowException()
        {
            var squareClient = this.GivenSquareClientCatalogApiWaitsToContinue(out var waitHandle);
            var cts = new CancellationTokenSource();
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(new MandarinConfiguration()));
            var task = subject.GetInventory().ToList().ToTask(cts.Token);
            cts.Cancel();
            waitHandle.Set();
            Assert.ThrowsAsync<TaskCanceledException>(() => task);
        }

        [Test]
        public async Task GetInventory_WhenServiceListsMultiplePages_ShouldContainAllObjects()
        {
            var squareClient = this.GivenSquareClientCatalogApiReturnsData();
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(new MandarinConfiguration()));
            var catalogObjects = await subject.GetInventory().ToList().ToTask((CancellationToken)default);
            Assert.That(catalogObjects.Count, Is.EqualTo(2));
            Assert.That(catalogObjects[0].ProductCode, Is.EqualTo("ID-1"));
            Assert.That(catalogObjects[1].ProductCode, Is.EqualTo("ID-2"));
        }

        private async Task<string> GivenTemporaryFileExists(FixedCommissionAmount data)
        {
            var filename = Path.GetTempFileName();
            var json = $"[{{ \"product_code\": \"{data.ProductCode}\", \"amount\": \"{data.Amount}\"}}]";
            await File.WriteAllTextAsync(filename, json);
            return filename;
        }

        private ISquareClient GivenSquareClientCatalogApiReturnsData()
        {
            var squareClient = new Mock<ISquareClient>();
            squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(null, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage1));
            squareClient.Setup(x => x.CatalogApi.ListCatalogAsync(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2, "ITEM", It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.DeserializeFromFile<ListCatalogResponse>(WellKnownTestData.Square.CatalogApi.ListCatalog.ItemsOnlyPage2));

            return squareClient.Object;
        }

        private ISquareClient GivenSquareClientCatalogApiWaitsToContinue(out ManualResetEvent waitHandle)
        {
            var mre = new ManualResetEvent(false);
            var squareClient = new Mock<ISquareClient>();
            squareClient
                .Setup(x => x.CatalogApi.ListCatalogAsync(null, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    mre.WaitOne();
                    return new ListCatalogResponse();
                }));

            waitHandle = mre;
            return squareClient.Object;
        }
    }
}
