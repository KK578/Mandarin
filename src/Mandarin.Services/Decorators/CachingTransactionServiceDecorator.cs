using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Square.Models;

namespace Mandarin.Services.Decorators
{
    internal sealed class CachingTransactionServiceDecorator : ITransactionService
    {
        private const string CacheKey = nameof(ITransactionService) + "." + nameof(CachingTransactionServiceDecorator.GetAllTransactions);
        private readonly ITransactionService transactionService;
        private readonly IMemoryCache memoryCache;

        public CachingTransactionServiceDecorator(ITransactionService transactionService, IMemoryCache memoryCache)
        {
            this.transactionService = transactionService;
            this.memoryCache = memoryCache;
        }

        public IObservable<Order> GetAllTransactions(DateTime start, DateTime end)
        {
            return Observable
                   .FromAsync(() => this.memoryCache.GetOrCreateAsync(CachingTransactionServiceDecorator.CreateCacheKey(start, end), CreateEntry))
                   .SelectMany(x => x);

            async Task<IReadOnlyList<Order>> CreateEntry(ICacheEntry e)
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
                    return new List<Order>().AsReadOnly();
                }
            }
        }

        private static string CreateCacheKey(DateTime start, DateTime end)
        {
            return $"{CachingTransactionServiceDecorator.CacheKey}-{start}-{end}";
        }
    }
}
