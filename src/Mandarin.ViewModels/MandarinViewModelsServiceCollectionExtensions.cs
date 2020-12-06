using System;
using System.IO;
using Mandarin.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register all services in this assembly.
    /// </summary>
    public static class MandarinViewModelsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Mandarin.ViewModels assembly implementations into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinViewModels(this IServiceCollection services)
        {
            services.AddTransient<IViewModelFactory, ViewModelFactory>();

            services.AddTransient(MandarinViewModelsServiceCollectionExtensions.CreatePageContentModel);

            return services;
        }

        private static PageContentModel CreatePageContentModel(IServiceProvider provider)
        {
            var configuration = provider.GetService<IOptions<MandarinConfiguration>>();
            using var fileStream = File.OpenRead(configuration.Value.PageContentFilePath);
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            var token = JToken.Load(jsonReader);
            return new PageContentModel(token);
        }
    }
}
