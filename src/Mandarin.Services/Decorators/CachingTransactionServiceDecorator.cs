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
    internal sealed class CachingTransactionServiceDecorator : ITransactionService
    {
        private const string CacheKey = nameof(ITransactionService) + "." + nameof(CachingTransactionServiceDecorator.GetAllTransactions);
        private readonly ITransactionService transactionService;
        private readonly IAppCache appCache;

        public CachingTransactionServiceDecorator(ITransactionService transactionService, IAppCache appCache)
        {
            this.transactionService = transactionService;
            this.appCache = appCache;
        }

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
