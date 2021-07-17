using System;
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
            public async Task ShouldNotBeAbleToExecuteWhenProductIsNotYetSelected()
            {
                var subject = this.Subject;
                subject.FramePrice.FramePrice = this.fixture.Create<decimal>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldNotBeAbleToExecuteWhenCommissionAmountIsNotYetSelected()
            {
                var subject = this.Subject;
                subject.SelectedProduct = this.fixture.Create<Product>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldBeAbleToExecuteWhenRequiredPropertiesAreSet()
            {
                var subject = this.Subject;
                subject.SelectedProduct = this.fixture.Create<Product>();
                subject.FramePrice.FramePrice = this.fixture.Create<decimal>();
                subject.FramePrice.CreatedAt = this.fixture.Create<DateTime>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldSaveFramePriceOnExecute()
            {
                var subject = this.Subject;
                var product = this.fixture.Create<Product>();
                subject.SelectedProduct = product;
                subject.FramePrice.FramePrice = 20.00M;
                subject.FramePrice.CreatedAt = new DateTime(2021, 06, 30);

                this.framePricesService.Setup(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                await subject.Save.Execute();
                this.framePricesService.Verify(x => x.SaveFramePriceAsync(new FramePrice
                {
                    ProductCode = product.ProductCode,
                    Amount = 20.00M,
                    CreatedAt = new DateTime(2021, 06, 30),
                }));
            }

            [Fact]
            public async Task ShouldNavigateToEditPageOnSavingSuccessfully()
            {
                var subject = this.Subject;
                var product = this.fixture.Create<Product>();
                subject.SelectedProduct = product;
                subject.FramePrice.FramePrice = this.fixture.Create<decimal>();
                subject.FramePrice.CreatedAt = this.fixture.Create<DateTime>();

                await subject.Save.Execute();

                this.navigationManager.Uri.Should().EndWith($"/inventory/frame-prices/edit/{product.ProductCode}");
            }
        }
    }
}
