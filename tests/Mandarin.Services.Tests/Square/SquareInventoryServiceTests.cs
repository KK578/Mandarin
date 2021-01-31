using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Configuration;
using Mandarin.Inventory;
using Mandarin.Services.Square;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Square;
using Square.Models;
using Xunit;

namespace Mandarin.Services.Tests.Square
{
    public class SquareInventoryServiceTests
    {
        [Fact]
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
            result.Should().HaveCount(2);
            result[1].ProductCode.Should().Be(additionalData.ProductCode);
        }

        [Fact]
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
            result.Should().HaveCount(1);
            result[0].ProductCode.Should().Be(additionalData.ProductCode);
            result[0].Amount.Should().Be(additionalData.Amount);
        }

        [Fact]
        public async Task DeleteFixedCommissionAmount_GivenDataExists_ShouldAppendValue()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));

            await subject.DeleteFixedCommissionAmount(data.ProductCode);

            var result = await subject.GetFixedCommissionAmounts().ToList();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFixedCommissionAmounts_GivenEmptyFileName_ShouldReturnEmpty()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = string.Empty };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFixedCommissionAmounts_GivenNonExistingFile_ShouldReturnEmpty()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = "NonExistentFile.json" };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFixedCommissionAmounts_GivenFileExists_ShouldContainAllObjects()
        {
            var squareClient = Mock.Of<ISquareClient>();
            var data = TestData.Create<FixedCommissionAmount>();
            var filename = await this.GivenTemporaryFileExists(data);
            var configuration = new MandarinConfiguration { FixedCommissionAmountFilePath = filename };
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(configuration));
            var actual = await subject.GetFixedCommissionAmounts().ToList().ToTask();

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(data);
        }

        [Fact]
        public void GetInventory_WhenRequestIsCancelled_ShouldThrowException()
        {
            var squareClient = this.GivenSquareClientCatalogApiWaitsToContinue(out var waitHandle);
            var cts = new CancellationTokenSource();
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(new MandarinConfiguration()));
            var task = subject.GetInventory().ToList().ToTask(cts.Token);
            cts.Cancel();
            waitHandle.Set();

            task.Awaiting(x => x).Should().ThrowAsync<TaskCanceledException>();
        }

        [Fact]
        public async Task GetInventory_WhenServiceListsMultiplePages_ShouldContainAllObjects()
        {
            var squareClient = this.GivenSquareClientCatalogApiReturnsData();
            var subject = new SquareInventoryService(NullLogger<SquareTransactionService>.Instance, squareClient, Options.Create(new MandarinConfiguration()));
            var catalogObjects = await subject.GetInventory().ToList().ToTask((CancellationToken)default);

            catalogObjects.Should().HaveCount(2);
            catalogObjects[0].ProductCode.Should().Be("ID-1");
            catalogObjects[0].ProductName.Should().Be("Item1 (Regular)");
            catalogObjects[1].ProductCode.Should().Be("ID-2");
            catalogObjects[1].ProductName.Should().Be("Item2 (Regular)");
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
            squareClient.Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.Is<SearchCatalogObjectsRequest>(request => request.Cursor == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page1);
            squareClient.Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.Is<SearchCatalogObjectsRequest>(request => request.Cursor == nameof(WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page2)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page2);

            return squareClient.Object;
        }

        private ISquareClient GivenSquareClientCatalogApiWaitsToContinue(out ManualResetEvent waitHandle)
        {
            var mre = waitHandle = new ManualResetEvent(false);
            var squareClient = new Mock<ISquareClient>();
            squareClient.Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.IsAny<SearchCatalogObjectsRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    mre.WaitOne();
                    return new SearchCatalogObjectsResponse();
                }));
            return squareClient.Object;
        }
    }
}
