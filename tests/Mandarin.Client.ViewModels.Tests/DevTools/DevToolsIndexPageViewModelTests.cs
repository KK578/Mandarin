using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.DevTools;
using Mandarin.Inventory;
using Mandarin.Transactions.External;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.DevTools
{
    public class DevToolsIndexPageViewModelTests
    {
        private static readonly DateTime Date = new(2021, 06, 01, 00, 00, 00, DateTimeKind.Utc);

        private readonly Mock<IProductSynchronizer> productSynchronizer = new();
        private readonly Mock<ITransactionSynchronizer> transactionSynchronizer = new();
        private readonly DevToolsIndexPageViewModel subject;

        protected DevToolsIndexPageViewModelTests()
        {
            this.subject = new DevToolsIndexPageViewModel(this.productSynchronizer.Object, this.transactionSynchronizer.Object);
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
                this.transactionSynchronizer.Setup(x => x.LoadExternalTransactions(DevToolsIndexPageViewModelTests.Date, DevToolsIndexPageViewModelTests.Date.AddDays(1)))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

                this.subject.StartDate = DevToolsIndexPageViewModelTests.Date;
                this.subject.EndDate = DevToolsIndexPageViewModelTests.Date.AddDays(1);
                await this.subject.SynchronizeTransactions.Execute();

                this.transactionSynchronizer.VerifyAll();
            }
        }
    }
}
