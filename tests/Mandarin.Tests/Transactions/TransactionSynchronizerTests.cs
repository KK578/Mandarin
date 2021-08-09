using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Mandarin.Database;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Tests.Helpers;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Transactions
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class TransactionSynchronizerTests : MandarinIntegrationTestsBase
    {
        private readonly ITransactionSynchronizer transactionSynchronizer;
        private readonly ITransactionRepository transactionRepository;

        public TransactionSynchronizerTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.transactionSynchronizer = fixture.Services.GetRequiredService<ITransactionSynchronizer>();
            this.transactionRepository = fixture.Services.GetRequiredService<ITransactionRepository>();
        }

        [Fact]
        public async Task ShouldProcessTransactionFromExternalTransactionCorrectly()
        {
            await this.GivenTransactionTableIsEmptyAsync();
            await this.transactionSynchronizer.SynchronizeTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));
            var transaction = await this.transactionRepository.GetTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));

            transaction.Should().MatchTransaction(WellKnownTestData.Transactions.Transaction1);
        }

        [Fact]
        public async Task ShouldNotProcessTheSameTransactionTwice()
        {
            await this.GivenTransactionTableIsEmptyAsync();
            await this.transactionSynchronizer.SynchronizeTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));
            await this.transactionSynchronizer.SynchronizeTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));

            var transactions = await this.transactionRepository.GetAllTransactionsAsync();
            transactions.Should().HaveCount(1);
        }

        [Fact]
        public async Task ShouldUpdateTheTransactionIfItIsInADifferentState()
        {
            var anotherInstant = MandarinFixture.Instance.NewInstant;
            await this.GivenTransactionTableIsEmptyAsync();
            await this.transactionRepository.SaveTransactionAsync(WellKnownTestData.Transactions.Transaction1 with { Timestamp = anotherInstant });
            await this.transactionSynchronizer.SynchronizeTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));
            var transaction = await this.transactionRepository.GetTransactionAsync(ExternalTransactionId.Of("sNVseFoHwzywEiVV69mNfK5eV"));

            transaction.Should().MatchTransaction(WellKnownTestData.Transactions.Transaction1);
        }

        private async Task GivenTransactionTableIsEmptyAsync()
        {
            var db = this.Fixture.Services.GetRequiredService<MandarinDbContext>();
            await db.GetConnection().ExecuteAsync(@"
                DELETE FROM billing.subtransaction;
                DELETE FROM billing.transaction;");
        }
    }
}
