using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Services.Square;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Represents common functionality shared by all caching decorators.
    /// </summary>
    internal abstract class CachingDecoratorBase
    {
        private readonly IAppCache appCache;
        private readonly ILogger<CachingDecoratorBase> logger;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingDecoratorBase"/> class.
        /// </summary>
        /// <param name="appCache">The application caching service.</param>
        /// <param name="logger">The application logger.</param>
        protected CachingDecoratorBase(IAppCache appCache, ILogger<CachingDecoratorBase> logger)
        {
            this.appCache = appCache;
            this.logger = logger;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Gets the existing values from the application cache, or creates it fresh from the provided factory method.
        /// </summary>
        /// <typeparam name="T">The type of the values to be cached.</typeparam>
        /// <param name="key">The key to identify the value in the cache.</param>
        /// <param name="addItemFactory">The factory method to create a new instance.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        protected Task<IReadOnlyList<T>> GetOrAddAsync<T>(string key, Func<Task<IEnumerable<T>>> addItemFactory)
        {
            return this.appCache.GetOrAddAsync(key, AddItemFactory, DateTimeOffset.Now.AddHours(1));

            async Task<IReadOnlyList<T>> AddItemFactory()
            {
                try
                {
                    await this.semaphore.WaitAsync();
                    var result = (await addItemFactory()).NullToEmpty().ToList().AsReadOnly();
                    this.logger.LogInformation("Adding Cache Entry '{Key}' with {Count} Items", key, result.Count);

                    var existingKeys = await this.appCache.GetAsync<List<string>>("Cache.Keys");
                    var keys = existingKeys.NullToEmpty().Append(key).Distinct().ToList();
                    this.appCache.Add("Cache.Keys", keys, DateTimeOffset.MaxValue);

                    return result;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Adding Cache Entry '{Key}' failed.", key);
                    return Enumerable.Empty<T>().NullToEmpty().ToList().AsReadOnly();
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
        }
    }
}
