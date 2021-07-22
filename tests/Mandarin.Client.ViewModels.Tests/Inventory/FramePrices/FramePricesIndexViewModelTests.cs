using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Inventory;
using Mandarin.Tests.Data;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePricesIndexViewModelTests
    {
        private static readonly FramePrice FramePrice = new()
        {
            ProductCode = new ProductCode("TLM-001"),
            Amount = 15.00M,
        };

        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IProductRepository> productRepository = new();
        private readonly MockNavigationManager navigationManager = new();

        private IFramePricesIndexViewModel Subject =>
            new FramePricesIndexViewModel(this.framePricesService.Object, this.productRepository.Object, this.navigationManager);

        public class IsLoadingTests : FramePricesIndexViewModelTests
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
                var unused = subject.LoadData.Execute().ToTask();

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

        public class RowsTests : FramePricesIndexViewModelTests
        {
            [Fact]
            public void ShouldNotBeNullOnConstruction()
            {
                this.Subject.Rows.Should().NotBeNull();
            }

            [Fact]
            public async Task ShouldBePresentAfterLoadDataIsExecuted()
            {
                var data = new List<FramePrice> { FramePricesIndexViewModelTests.FramePrice };
                this.framePricesService.Setup(x => x.GetAllFramePricesAsync()).ReturnsAsync(data);
                this.productRepository
                    .Setup(x => x.GetProductByCodeAsync(FramePricesIndexViewModelTests.FramePrice.ProductCode))
                    .ReturnsAsync(WellKnownTestData.Products.Mandarin);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Rows.Should().HaveCount(1);
            }
        }

        public class CreateNewTests : FramePricesIndexViewModelTests
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

        public class EditSelectedTests : FramePricesIndexViewModelTests
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
                subject.SelectedRow = Mock.Of<IFramePriceViewModel>();
                var result = await subject.EditSelected.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRedirectOnExecute()
            {
                var subject = this.Subject;
                subject.SelectedRow = Mock.Of<IFramePriceViewModel>(x => x.ProductCode == "TLM-001");
                await subject.EditSelected.Execute();
                this.navigationManager.Uri.Should().Contain("/inventory/frame-prices/edit/TLM-001");
            }
        }
    }
}
