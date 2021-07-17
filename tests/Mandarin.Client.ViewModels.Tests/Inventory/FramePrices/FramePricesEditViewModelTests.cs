using System;
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
    public class FramePricesEditViewModelTests
    {
        private static readonly DateTime OriginalCommissionDate = new(2021, 06, 10);

        private readonly Fixture fixture = new();
        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IQueryableProductService> productService = new();
        private readonly NavigationManager navigationManager = new MockNavigationManager();

        private IFramePricesEditViewModel Subject =>
            new FramePricesEditViewModel(this.framePricesService.Object, this.productService.Object, this.navigationManager);

        private void GivenServicesReturnProduct(Product product, decimal? commission = null)
        {
            commission ??= this.fixture.Create<decimal>();

            this.productService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
            this.framePricesService.Setup(x => x.GetFramePriceAsync(product.ProductCode, It.IsAny<DateTime>()))
                .ReturnsAsync(new FramePrice
                {
                    ProductCode = product.ProductCode,
                    Amount = commission.Value,
                    CreatedAt = FramePricesEditViewModelTests.OriginalCommissionDate,
                });
        }


        public class IsLoadingTests : FramePricesEditViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.Subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<Product>();
                this.productService.Setup(x => x.GetProductByProductCodeAsync(It.IsAny<ProductCode>())).Returns(tcs.Task);

                var subject = this.Subject;
                var executingTask = subject.LoadData.Execute(new ProductCode("TLM-001")).ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldHaveAvailableListOfProductsAfterLoad()
            {
                var product = this.fixture.Create<Product>();
                this.productService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
                this.framePricesService.Setup(x => x.GetFramePriceAsync(product.ProductCode, It.IsAny<DateTime>()))
                    .ReturnsAsync(this.fixture.Create<FramePrice>());

                var subject = this.Subject;
                await subject.LoadData.Execute(product.ProductCode);

                subject.Product.Should().Be(product);
            }
        }

        public class AmountTests : FramePricesEditViewModelTests
        {
            [Fact]
            public void StockistAmountShouldAutomaticallyUpdateOnAmountChanging()
            {
                var product = this.fixture.Create<Product>() with { UnitPrice = 150.00M };
                this.GivenServicesReturnProduct(product, 10.00M);

                var subject = this.Subject;

                subject.LoadData.Execute(product.ProductCode);
                subject.StockistAmount.Should().Be(140.00M);

                subject.FrameAmount = 20.00M;
                subject.StockistAmount.Should().Be(130.00M);
            }
        }

        public class CreatedAtTests : FramePricesEditViewModelTests
        {
            [Fact]
            public void CreatedAtShouldAutomaticallyUpdateOnChangingCommissionAmount()
            {
                var product = this.fixture.Create<Product>() with { UnitPrice = 150.00M };
                this.GivenServicesReturnProduct(product, 10.00M);

                var subject = this.Subject;

                subject.LoadData.Execute(product.ProductCode);
                subject.CreatedAt.Should().BeCloseTo(FramePricesEditViewModelTests.OriginalCommissionDate);

                subject.FrameAmount = 20.00M;
                subject.CreatedAt.Should().NotBeCloseTo(FramePricesEditViewModelTests.OriginalCommissionDate);
                subject.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));

                subject.FrameAmount = 10.00M;
                subject.CreatedAt.Should().BeCloseTo(FramePricesEditViewModelTests.OriginalCommissionDate);
            }
        }

        public class CancelTests : FramePricesEditViewModelTests
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

        public class SaveTests : FramePricesEditViewModelTests
        {
            private readonly Product product;

            public SaveTests()
            {
                this.product = this.fixture.Create<Product>();
            }

            [Fact]
            public async Task ShouldNotBeExecutableOnConstruction()
            {
                var canExecute = await this.Subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeFalse();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public async Task ShouldNotBeAbleToExecuteWhenAnyRequiredPropertiesIsNotSet(int i)
            {
                var subject = this.Subject;
                if (i != 1)
                {
                    subject.FrameAmount = this.fixture.Create<decimal>();
                }

                if (i != 2)
                {
                    subject.CreatedAt = this.fixture.Create<DateTime>();
                }

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldBeAbleToExecuteWhenProductAndCommissionAreSet()
            {
                this.GivenServicesReturnProduct(this.product);

                var subject = this.Subject;
                await subject.LoadData.Execute(this.product.ProductCode);
                subject.FrameAmount = this.fixture.Create<decimal>();
                subject.CreatedAt = this.fixture.Create<DateTime>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldSaveCommissionOnExecute()
            {
                this.GivenServicesReturnProduct(this.product);

                var subject = this.Subject;
                await subject.LoadData.Execute(this.product.ProductCode);
                subject.FrameAmount = 20.00M;
                subject.CreatedAt = new DateTime(2021, 06, 30);

                this.framePricesService.Setup(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                await subject.Save.Execute();
                this.framePricesService.Verify(x => x.SaveFramePriceAsync(new FramePrice
                {
                    ProductCode = this.product.ProductCode,
                    Amount = 20.00M,
                    CreatedAt = new DateTime(2021, 06, 30),
                }));
            }

            [Fact]
            public async Task ShouldRedirectAfterSavingCommission()
            {
                this.GivenServicesReturnProduct(this.product);

                var subject = this.Subject;
                await subject.LoadData.Execute(this.product.ProductCode);
                subject.FrameAmount = this.fixture.Create<decimal>();
                subject.CreatedAt = this.fixture.Create<DateTime>();

                this.framePricesService.Setup(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>())).Returns(Task.CompletedTask);

                await subject.Save.Execute();
                this.navigationManager.Uri.Should().EndWith("/inventory/frame-prices");
            }
        }
    }
}
