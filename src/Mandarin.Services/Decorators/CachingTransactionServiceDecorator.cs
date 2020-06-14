using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Models.Transactions;
using Microsoft.Extensions.Caching.Memory;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Implementation of <see cref="ITransactionService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingTransactionServiceDecorator : ITransactionService
    {
        private const string CacheKey = nameof(ITransactionService) + "." + nameof(CachingTransactionServiceDecorator.GetAllTransactions);
        private readonly ITransactionService transactionService;
        private readonly IAppCache appCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingTransactionServiceDecorator"/> class.
        /// </summary>
        /// <param name="transactionService">The transactions service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        public CachingTransactionServiceDecorator(ITransactionService transactionService, IAppCache appCache)
        {
            this.transactionService = transactionService;
            this.appCache = appCache;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end)
        {
            return Observable
                   .FromAsync(() => this.appCache.GetOrAddAsync(CachingTransactionServiceDecorator.CreateCacheKey(start, end), CreateEntry))
                   .SelectMany(x => x);

            async Task<IReadOnlyList<Transaction>> CreateEntry(ICacheEntry e)
            {
                try
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var result = await this.transactionService.GetAllTransactions(start, end).ToList().ToTask();
                    return result.ToList().AsReadOnly();
                }
                catch (Exception)
                {
                    e.AbsoluteExpiration = DateTimeOffset.MinValue;
                    return new List<Transaction>().AsReadOnly();
                }
            }
        }

        private static string CreateCacheKey(DateTime start, DateTime end)
        {
            return $"{CachingTransactionServiceDecorator.CacheKey}-{start}-{end}";
        }
    }
}
