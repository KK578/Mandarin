using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Google.Type;
using Mandarin.Api.Transactions;
using Mandarin.Transactions;
using NodaTime;
using static Mandarin.Api.Transactions.Transactions;
using TransactionSummary=Mandarin.Transactions.TransactionSummary;

namespace Mandarin.Client.Services.Transactions
{
    /// <inheritdoc />
    public class MandarinGrpcTransactionSummaryRepository : ITransactionSummaryRepository
    {
        private readonly IMapper mapper;
        private readonly TransactionsClient transactionsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcTransactionSummaryRepository"/> class.
        /// </summary>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="transactionsClient">The gRPC client to Mandarin API for Transactions.</param>
        public MandarinGrpcTransactionSummaryRepository(IMapper mapper, TransactionsClient transactionsClient)
        {
            this.mapper = mapper;
            this.transactionsClient = transactionsClient;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TransactionSummary>> GetTransactionSummariesAsync(DateInterval interval)
        {
            var response = await this.transactionsClient.GetTransactionSummariesAsync(new GetTransactionSummariesRequest
            {
                Start = this.mapper.Map<Date>(interval.Start),
                End = this.mapper.Map<Date>(interval.End),
            });

            return this.mapper.Map<List<TransactionSummary>>(response.Summaries);
        }
    }
}
