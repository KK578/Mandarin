using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FixedCommissions;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Inventory;
using Mandarin.Tests.Data.Extensions;
using Microsoft.AspNetCore.Components;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FixedCommissions
{
    public class FixedCommissionsEditViewModelTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IFixedCommissionService> fixedCommissionService = new();
        private readonly Mock<IQueryableProductService> productService = new();
        private readonly NavigationManager navigationManager = new MockNavigationManager();

        private IFixedCommissionsEditViewModel Subject =>
            new FixedCommissionsEditViewModel(this.fixedCommissionService.Object,
                                             this.productService.Object,
                                             this.navigationManager);

        private void GivenServicesReturnProduct(Product product, decimal? commission = null)
        {
            commission ??= this.fixture.Create<decimal>();

            this.productService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
            this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync(product.ProductCode))
                .ReturnsAsync(new FixedCommissionAmount(product.ProductCode, commission.Value));
        }


        public class IsLoadingTests : FixedCommissionsEditViewModelTests
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
                this.productService.Setup(x => x.GetProductByProductCodeAsync(string.Empty)).Returns(tcs.Task);

                var subject = this.Subject;
                var executingTask = subject.LoadData.Execute(string.Empty).ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldHaveAvailableListOfProductsAfterLoad()
            {
                var product = this.fixture.Create<Product>();
                this.productService.Setup(x => x.GetProductByProductCodeAsync(product.ProductCode)).ReturnsAsync(product);
                this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync(product.ProductCode))
                    .ReturnsAsync(this.fixture.Create<FixedCommissionAmount>());

                var subject = this.Subject;
                await subject.LoadData.Execute(product.ProductCode);

                subject.Product.Should().Be(product);
            }
        }

        public class AmountTests : FixedCommissionsEditViewModelTests
        {
            [Fact]
            public void StockistAmountShouldAutomaticallyUpdateOnAmountChanging()
            {
                var product = this.fixture.Create<Product>().WithUnitPrice(150.00M);
                this.GivenServicesReturnProduct(product, 10.00M);

                var subject = this.Subject;

                subject.LoadData.Execute(product.ProductCode);
                subject.StockistAmount.Should().Be(140.00M);

                subject.CommissionAmount = 20.00M;
                subject.StockistAmount.Should().Be(130.00M);
            }
        }

        public class CancelTests : FixedCommissionsEditViewModelTests
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
                this.navigationManager.Uri.Should().EndWith("/inventory/fixed-commissions");
            }
        }

        public class SaveTests : FixedCommissionsEditViewModelTests
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

            [Fact]
            public async Task ShouldBeAbleToExecuteWhenProductAndCommissionAreSet()
            {
                this.GivenServicesReturnProduct(this.product);

                var subject = this.Subject;
                await subject.LoadData.Execute(this.product.ProductCode);
                subject.CommissionAmount = this.fixture.Create<decimal>();

                var canExecute = await subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldSaveCommissionOnExecute()
            {
                this.GivenServicesReturnProduct(this.product);

                var subject = this.Subject;
                await subject.LoadData.Execute(this.product.ProductCode);
                subject.CommissionAmount = 20.00M;

                this.fixedCommissionService.Setup(x => x.SaveFixedCommissionAsync(It.IsAny<FixedCommissionAmount>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                await subject.Save.Execute();
                this.fixedCommissionService.Verify(x => x.SaveFixedCommissionAsync(It.Is<FixedCommissionAmount>(amount =>
                                                                                       amount.ProductCode == this.product.ProductCode &&
                                                                                       amount.Amount == 20.00M)));
            }
        }
    }
}
