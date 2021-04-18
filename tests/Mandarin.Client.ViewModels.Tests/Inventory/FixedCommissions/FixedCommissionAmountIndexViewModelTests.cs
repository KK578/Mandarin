using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FixedCommissions;
using Mandarin.Inventory;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FixedCommissions
{
    public class FixedCommissionAmountIndexViewModelTests
    {
        private static readonly FixedCommissionAmount FixedCommissionAmount = new("TLM-001", 15.00M);
        private static readonly Product Product = new("SquareId", "TLM-001", "Mandarin", "It's a Mandarin!", 45.00M);

        private readonly Mock<IFixedCommissionService> fixedCommissionService = new();
        private readonly Mock<IQueryableProductService> productService = new();

        private IFixedCommissionAmountIndexViewModel Subject =>
            new FixedCommissionAmountIndexViewModel(this.fixedCommissionService.Object, this.productService.Object);

        public class IsLoadingTests : FixedCommissionAmountIndexViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.Subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<FixedCommissionAmount>>();
                this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync()).Returns(tcs.Task);

                var subject = this.Subject;
                var executingTask = subject.LoadData.Execute().ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldBeFalseAfterLoadingFinishes()
            {
                this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync()).ReturnsAsync(new List<FixedCommissionAmount>());

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.IsLoading.Should().BeFalse();
            }
        }

        public class RowsTests : FixedCommissionAmountIndexViewModelTests
        {
            [Fact]
            public void ShouldNotBeNullOnConstruction()
            {
                this.Subject.Rows.Should().NotBeNull();
            }

            [Fact]
            public async Task ShouldBePresentAfterLoadDataIsExecuted()
            {
                var data = new List<FixedCommissionAmount> { FixedCommissionAmountIndexViewModelTests.FixedCommissionAmount };
                this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync()).ReturnsAsync(data);
                this.productService
                    .Setup(x => x.GetProductByProductCodeAsync(FixedCommissionAmountIndexViewModelTests.FixedCommissionAmount.ProductCode))
                    .ReturnsAsync(FixedCommissionAmountIndexViewModelTests.Product);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Rows.Should().HaveCount(1);
            }
        }
    }
}
