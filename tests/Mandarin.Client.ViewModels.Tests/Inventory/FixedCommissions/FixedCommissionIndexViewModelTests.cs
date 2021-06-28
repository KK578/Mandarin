using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Inventory;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FixedCommissions
{
    public class FixedCommissionIndexViewModelTests
    {
        private static readonly FramePrice FramePrice = new("TLM-001", 15.00M);
        private static readonly Product Product = new("SquareId", "TLM-001", "Mandarin", "It's a Mandarin!", 45.00M);

        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IQueryableProductService> productService = new();
        private readonly MockNavigationManager navigationManager = new();

        private IFramePricesIndexViewModel Subject =>
            new FramePricesIndexViewModel(this.framePricesService.Object, this.productService.Object, this.navigationManager);

        public class IsLoadingTests : FixedCommissionIndexViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.Subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<FramePrice>>();
                this.framePricesService.Setup(x => x.GetAllFramePricesAsync()).Returns(tcs.Task);

                var subject = this.Subject;
                var executingTask = subject.LoadData.Execute().ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldBeFalseAfterLoadingFinishes()
            {
                this.framePricesService.Setup(x => x.GetAllFramePricesAsync()).ReturnsAsync(new List<FramePrice>());

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.IsLoading.Should().BeFalse();
            }
        }

        public class RowsTests : FixedCommissionIndexViewModelTests
        {
            [Fact]
            public void ShouldNotBeNullOnConstruction()
            {
                this.Subject.Rows.Should().NotBeNull();
            }

            [Fact]
            public async Task ShouldBePresentAfterLoadDataIsExecuted()
            {
                var data = new List<FramePrice> { FixedCommissionIndexViewModelTests.FramePrice };
                this.framePricesService.Setup(x => x.GetAllFramePricesAsync()).ReturnsAsync(data);
                this.productService
                    .Setup(x => x.GetProductByProductCodeAsync(FixedCommissionIndexViewModelTests.FramePrice.ProductCode))
                    .ReturnsAsync(FixedCommissionIndexViewModelTests.Product);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Rows.Should().HaveCount(1);
            }
        }

        public class CreateNewTests : FixedCommissionIndexViewModelTests
        {
            [Fact]
            public async Task ShouldBeExecutableOnConstruction()
            {
                var result = await this.Subject.CreateNew.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRedirectOnExecute()
            {
                await this.Subject.CreateNew.Execute();
                this.navigationManager.Uri.Should().Contain("/inventory/frame-prices/new");
            }
        }

        public class EditSelectedTests : FixedCommissionIndexViewModelTests
        {
            [Fact]
            public async Task ShouldNotBeExecutableOnConstruction()
            {
                var result = await this.Subject.EditSelected.CanExecute.FirstAsync();
                result.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldBeExecutableOnceARowIsSelected()
            {
                var subject = this.Subject;
                subject.SelectedRow = Mock.Of<IFramePricesGridRowViewModel>();
                var result = await subject.EditSelected.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRedirectOnExecute()
            {
                var subject = this.Subject;
                subject.SelectedRow = Mock.Of<IFramePricesGridRowViewModel>(x => x.ProductCode == "TLM-001");
                await subject.EditSelected.Execute();
                this.navigationManager.Uri.Should().Contain("/inventory/frame-prices/edit/TLM-001");
            }
        }
    }
}
