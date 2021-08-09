using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Google.Type;
using Mandarin.Api.Transactions;
using Mandarin.Transactions.External;
using NodaTime;
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
        public Task LoadExternalTransactionsInPastDay() => throw MandarinGrpcTransactionSynchronizer.UnsupportedException();

        /// <inheritdoc />
        public Task LoadExternalTransactionsInPast2Months() => throw MandarinGrpcTransactionSynchronizer.UnsupportedException();

        /// <inheritdoc />
        public async Task LoadExternalTransactions(LocalDate start, LocalDate end)
        {
            var request = new SynchronizeTransactionsRequest
            {
                Start = this.mapper.Map<Date>(start),
                End = this.mapper.Map<Date>(end),
            };
            await this.transactionsClient.SynchronizeTransactionsAsync(request);
        }

        /// <inheritdoc />
        public Task SynchronizeTransactionAsync(ExternalTransactionId externalTransactionId) => throw MandarinGrpcTransactionSynchronizer.UnsupportedException();

        private static Exception UnsupportedException([CallerMemberName] string callerMethod = null)
        {
            return new NotSupportedException($"Mandarin API does not support {callerMethod}.");
        }
    }
}
