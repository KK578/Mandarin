using System;
using Mandarin.App.Commands.Stockists;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.App
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to help with registrations of <see cref="Lazy{T}"/>.
    /// </summary>
    internal static class LazyServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> to the
        /// specified <see cref="IServiceCollection"/>, and additionally registers a <see cref="Lazy{T}"/>
        /// for <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddLazyTransient<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddTransient<TService>();
            services.AddTransient(s => new Lazy<TService>(s.GetService<TService>));

            return services;
        }
    }
}
