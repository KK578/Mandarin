using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Hangfire;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.AspNetCore.Authorization;
using NodaTime;
using static Mandarin.Api.Transactions.Transactions;
using Transaction = Mandarin.Api.Transactions;
using TransactionSummary = Mandarin.Api.Transactions.TransactionSummary;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class TransactionsGrpcService : TransactionsBase
    {
        private readonly IMapper mapper;
        private readonly IBackgroundJobClient jobs;
        private readonly ITransactionSummaryRepository transactionSummaryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsGrpcService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="jobs">The service to interact with background jobs.</param>
        /// <param name="transactionSummaryRepository">The service to interact with <see cref="TransactionSummary"/> items.</param>
        public TransactionsGrpcService(IMapper mapper, IBackgroundJobClient jobs, ITransactionSummaryRepository transactionSummaryRepository)
        {
            this.mapper = mapper;
            this.jobs = jobs;
            this.transactionSummaryRepository = transactionSummaryRepository;
        }

        /// <inheritdoc />
        public override Task<Transaction.SynchronizeTransactionsResponse> SynchronizeTransactions(Transaction.SynchronizeTransactionsRequest request, ServerCallContext context)
        {
            var start = this.mapper.Map<LocalDate>(request.Start);
            var end = this.mapper.Map<LocalDate>(request.End);
            this.jobs.Enqueue<ITransactionSynchronizer>(s => s.LoadExternalTransactions(start, end));

            return Task.FromResult(new Transaction.SynchronizeTransactionsResponse());
        }

        /// <inheritdoc/>
        public override async Task<Transaction.GetTransactionSummariesResponse> GetTransactionSummaries(Transaction.GetTransactionSummariesRequest request, ServerCallContext context)
        {
            var start = this.mapper.Map<LocalDate>(request.Start);
            var end = this.mapper.Map<LocalDate>(request.End);

            var transactionSummaries = await this.transactionSummaryRepository.GetTransactionSummariesAsync(new DateInterval(start, end));
            return new Transaction.GetTransactionSummariesResponse
            {
                Summaries = { this.mapper.Map<List<TransactionSummary>>(transactionSummaries) },
            };
        }
    }
}
