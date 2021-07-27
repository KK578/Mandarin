using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using LazyCache;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Common
{
    /// <summary>
    /// Implementation of <see cref="ITransactionService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingTransactionServiceDecorator : CachingDecoratorBase, ITransactionService
    {
        private readonly ITransactionService transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingTransactionServiceDecorator"/> class.
        /// </summary>
        /// <param name="transactionService">The transactions service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        /// <param name="logger">The application logger.</param>
        public CachingTransactionServiceDecorator(ITransactionService transactionService, IAppCache appCache, ILogger<CachingTransactionServiceDecorator> logger)
            : base(appCache, logger)
        {
            this.transactionService = transactionService;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end)
        {
            var key = CachingTransactionServiceDecorator.CreateCacheKey(start, end);
            return this.GetOrAddAsync(key, () => this.transactionService.GetAllTransactions(start, end).ToList().ToTask<IEnumerable<Transaction>>())
                       .ToObservable()
                       .Catch((Exception ex) => Observable.Empty<IEnumerable<Transaction>>())
                       .SelectMany(x => x);
        }

        private static string CreateCacheKey(DateTime start, DateTime end, [CallerMemberName] string methodName = null)
        {
            return $"{nameof(ITransactionService)}-{methodName}-{start}-{end}";
        }
    }
}
