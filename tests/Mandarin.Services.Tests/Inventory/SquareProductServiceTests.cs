using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
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
        private readonly Mock<ISquareClient> squareClient = new();
        private readonly ISquareProductService subject;

        protected SquareProductServiceTests()
        {
            this.subject = new SquareProductService(NullLogger<SquareProductService>.Instance,
                                                    this.squareClient.Object);
        }

        private void GivenSquareClientCatalogApiReturnsData()
        {
            this.squareClient.Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.Is<SearchCatalogObjectsRequest>(request => request.Cursor == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page1);
            this.squareClient.Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.Is<SearchCatalogObjectsRequest>(request => request.Cursor == nameof(WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page2)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => WellKnownTestData.Square.CatalogApi.SearchCatalogObjects.Page2);
        }

        private ManualResetEvent GivenSquareClientCatalogApiWaitsToContinue()
        {
            var waitHandle = new ManualResetEvent(false);
            this.squareClient
                .Setup(x => x.CatalogApi.SearchCatalogObjectsAsync(It.IsAny<SearchCatalogObjectsRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.Run(() =>
                {
                    waitHandle.WaitOne();
                    return new SearchCatalogObjectsResponse();
                }));
            return waitHandle;
        }

        public class GetAllProductsAsyncTests : SquareProductServiceTests
        {
            [Fact]
            public void ShouldThrowExceptionWhenRequestFails()
            {
                var waitHandle = this.GivenSquareClientCatalogApiWaitsToContinue();
                var cts = new CancellationTokenSource();
                var task = this.subject.GetAllProductsAsync();
                cts.Cancel();
                waitHandle.Set();

                task.Awaiting(x => x).Should().ThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task ShouldContainCompleteListOverAllPages()
            {
                this.GivenSquareClientCatalogApiReturnsData();
                var catalogObjects = await this.subject.GetAllProductsAsync();

                catalogObjects.Should().HaveCount(2);
                catalogObjects[0].ProductCode.Should().Be(ProductCode.Of("ID-1"));
                catalogObjects[0].ProductName.Should().Be(ProductName.Of("Item1 (Regular)"));
                catalogObjects[1].ProductCode.Should().Be(ProductCode.Of("ID-2"));
                catalogObjects[1].ProductName.Should().Be(ProductName.Of("Item2 (Regular)"));
            }
        }
    }
}
