using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Inventory;
using Mandarin.Tests.Data.Extensions;
using Microsoft.AspNetCore.Components;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePricesNewViewModelTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IQueryableProductService> productService = new();
        private readonly NavigationManager navigationManager = new MockNavigationManager();

        private IFramePricesNewViewModel Subject =>
            new FramePricesNewViewModel(this.framePricesService.Object, this.productService.Object, this.navigationManager);

        public class IsLoadingTests : FramePricesNewViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.Subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<Product>>();
                this.productService.Setup(x => x.GetAllProductsAsync()).Returns(tcs.Task);

                var subject = this.Subject;
                var executingTask = subject.LoadData.Execute().ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldHaveAvailableListOfProductsAfterLoad()
            {
                var products = this.fixture.CreateMany<Product>().ToList().AsReadOnly();
                this.productService.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Products.Should().BeEquivalentTo(products);
            }
        }

        public class AmountTests : FramePricesNewViewModelTests
        {
            [Fact]
            public void StockistAmountShouldAutomaticallyUpdateOnAmountChanging()
            {
                var product = this.fixture.Create<Product>().WithUnitPrice(150.00M);
                var subject = this.Subject;

                subject.SelectedProduct = product;
                subject.FrameAmount = 10.00M;
                subject.StockistAmount.Should().Be(140.00M);

                subject.FrameAmount = 20.00M;
                subject.StockistAmount.Should().Be(130.00M);
            }
        }

        public class CancelTests : FramePricesNewViewModelTests
        {
            [Fact]
            public async Task ShouldBeExecutableOnConstruction()
            {
                var canExecute = await this.Subject.Cancel.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldNavigateToIndexOnCancel()
            {
                await this.Subject.Cancel.Execute();
                this.navigationManager.Uri.Should().EndWith("/inventory/frame-prices");
            }
        }

        public class SaveTests : FramePricesNewViewModelTests
        {
            [Fact]
            public async Task ShouldNotBeExecutableOnConstruction()
            {
                var canExecute = await this.Subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldBeAbleToExecuteWhenProductAndCommissionAreSet()
            {
                var subject = this.Subject;
                subject.SelectedProduct = this.fixture.Create<Product>();
                subject.FrameAmount = this.fixture.Create<decimal>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldSaveFramePriceOnExecute()
            {
                var subject = this.Subject;
                var product = this.fixture.Create<Product>();
                subject.SelectedProduct = product;
                subject.FrameAmount = 20.00M;

                this.framePricesService.Setup(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                await subject.Save.Execute();
                this.framePricesService.Verify(x => x.SaveFramePriceAsync(It.Is<FramePrice>(amount =>
                                                                                       amount.ProductCode == product.ProductCode &&
                                                                                       amount.Amount == 20.00M)));
            }

            [Fact]
            public async Task ShouldNavigateToEditPageOnSavingSuccessfully()
            {
                var subject = this.Subject;
                var product = this.fixture.Create<Product>();
                subject.SelectedProduct = product;
                subject.FrameAmount = this.fixture.Create<decimal>();

                await subject.Save.Execute();

                this.navigationManager.Uri.Should().EndWith($"/inventory/frame-prices/edit/{product.ProductCode}");
            }
        }
    }
}
