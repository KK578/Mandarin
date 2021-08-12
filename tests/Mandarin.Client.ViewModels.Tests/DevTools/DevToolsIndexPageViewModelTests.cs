using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.ViewModels.DevTools;
using Mandarin.Inventory;
using Mandarin.Transactions.External;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.DevTools
{
    public class DevToolsIndexPageViewModelTests
    {
        private static readonly LocalDate Start = new(2021, 06, 01);
        private static readonly LocalDate End = new(2021, 07, 01);

        private readonly Mock<IProductSynchronizer> productSynchronizer = new();
        private readonly Mock<ITransactionSynchronizer> transactionSynchronizer = new();
        private readonly IClock clock = FakeClock.FromUtc(2021, 08, 09);
        private readonly DevToolsIndexPageViewModel subject;

        public DevToolsIndexPageViewModelTests()
        {
            this.subject = new DevToolsIndexPageViewModel(this.productSynchronizer.Object, this.transactionSynchronizer.Object, this.clock);
        }

        [Fact]
        public void ShouldInstantiateWithStartAndEndTimes()
        {
            this.subject.StartDate.Should().Be(new LocalDate(2021, 07, 09));
            this.subject.EndDate.Should().Be(new LocalDate(2021, 08, 09));
        }

        public class SynchronizeProductTests : DevToolsIndexPageViewModelTests
        {
            [Fact]
            public async Task ShouldInvokeProductSynchronizer()
            {
                this.productSynchronizer.Setup(x => x.SynchronizeProductsAsync()).Returns(Task.CompletedTask).Verifiable();
                await this.subject.SynchronizeProducts.Execute();
                this.productSynchronizer.VerifyAll();
            }
        }

        public class SynchronizeTransactionTests : DevToolsIndexPageViewModelTests
        {
            [Fact]
            public async Task ShouldEnqueueCorrectJobOnSynchronizeTransaction()
            {
                this.transactionSynchronizer.Setup(x => x.LoadExternalTransactions(DevToolsIndexPageViewModelTests.Start, DevToolsIndexPageViewModelTests.End))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                this.subject.StartDate = DevToolsIndexPageViewModelTests.Start;
                this.subject.EndDate = DevToolsIndexPageViewModelTests.End;
                await this.subject.SynchronizeTransactions.Execute();

                this.transactionSynchronizer.VerifyAll();
            }
        }
    }
}
