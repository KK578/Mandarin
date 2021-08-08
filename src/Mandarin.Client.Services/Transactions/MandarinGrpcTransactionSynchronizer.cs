using System;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Mandarin.Api.Transactions;
using Mandarin.Transactions.External;
using static Mandarin.Api.Transactions.Transactions;

namespace Mandarin.Client.Services.Transactions
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcTransactionSynchronizer : ITransactionSynchronizer
    {
        private readonly IMapper mapper;
        private readonly TransactionsClient transactionsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcTransactionSynchronizer"/> class.
        /// </summary>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="transactionsClient">The gRPC client to Mandarin API for Transactions.</param>
        public MandarinGrpcTransactionSynchronizer(IMapper mapper, TransactionsClient transactionsClient)
        {
            this.mapper = mapper;
            this.transactionsClient = transactionsClient;
        }

        /// <inheritdoc />
        public async Task LoadExternalTransactions(DateTime start, DateTime end)
        {
            var request = new SynchronizeTransactionsRequest
            {
                Start = this.mapper.Map<Timestamp>(start),
                End = this.mapper.Map<Timestamp>(end),
            };
            await this.transactionsClient.SynchronizeTransactionsAsync(request);
        }

        /// <inheritdoc />
        public Task SynchronizeTransactionAsync(ExternalTransactionId externalTransactionId)
        {
            throw new NotSupportedException("Mandarin API does not support synchronizing on a transaction basis.");
        }
    }
}
