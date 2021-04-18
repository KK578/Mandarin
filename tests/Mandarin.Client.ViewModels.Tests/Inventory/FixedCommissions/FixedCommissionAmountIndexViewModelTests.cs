using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FixedCommissions;
using Mandarin.Inventory;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FixedCommissions
{
    public class FixedCommissionAmountIndexViewModelTests
    {
        private readonly Mock<IFixedCommissionService> fixedCommissionService;

        protected FixedCommissionAmountIndexViewModelTests()
        {
            this.fixedCommissionService = new Mock<IFixedCommissionService>();
        }

        private IFixedCommissionAmountIndexViewModel Subject => new FixedCommissionAmountIndexViewModel(this.fixedCommissionService.Object);

        public class RowsTests : FixedCommissionAmountIndexViewModelTests
        {
            [Fact]
            public void ShouldNotBeNullOnInitialisation()
            {
                this.Subject.Rows.Should().NotBeNull();
            }

            [Fact]
            public async Task ShouldBePresentAfterLoadDataIsExecuted()
            {
                var data = TestData.Create<List<FixedCommissionAmount>>();
                this.fixedCommissionService.Setup(x => x.GetFixedCommissionAsync()).ReturnsAsync(data);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Rows.Should().BeEquivalentTo(data);
            }
        }
    }
}
