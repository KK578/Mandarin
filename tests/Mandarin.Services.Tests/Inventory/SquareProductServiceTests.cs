using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Services.Inventory;
using Mandarin.Services.Transactions;
using Mandarin.Tests.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Square;
using Square.Models;
using Xunit;

namespace Mandarin.Services.Tests.Inventory
{
    public class SquareProductServiceTests
    {
        [Fact]
        public void GetInventory_WhenRequestIsCancelled_ShouldThrowException()
        {
            var squareClient = this.GivenSquareClientCatalogApiWaitsToContinue(out var waitHandle);
            var cts = new CancellationTokenSource();
            var subject = new SquareProductService(NullLogger<SquareTransactionService>.Instance, squareClient);
            var task = subject.GetAllProducts().ToList().ToTask(cts.Token);
            cts.Cancel();
            waitHandle.Set();

            task.Awaiting(x => x).Should().ThrowAsync<TaskCanceledException>();
        }

        [Fact]
        public async Task GetInventory_WhenServiceListsMultiplePages_ShouldContainAllObjects()
        {
            var squareClient = this.GivenSquareClientCatalogApiReturnsData();
            var subject = new SquareProductService(NullLogger<SquareTransactionService>.Instance, squareClient);
            var catalogObjects = await subject.GetAllProducts().ToList().ToTask((CancellationToken)default);

            catalogObjects.Should().HaveCount(2);
            catalogObjects[0].ProductCode.Should().Be("ID-1");
            catalogObjects[0].ProductName.Should().Be("Item1 (Regular)");
            catalogObjects[1].ProductCode.Should().Be("ID-2");
            catalogObjects[1].ProductName.Should().Be("Item2 (Regular)");
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
