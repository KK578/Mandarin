using Mandarin.Client.ViewModels.Index;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Client.ViewModels
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
            services.AddTransient<IIndexViewModel, IndexViewModel>();
            services.AddTransient<IFramePricesIndexViewModel, FramePricesIndexViewModel>();
            services.AddTransient<IFramePricesEditViewModel, FramePricesEditViewModel>();
            services.AddTransient<IFramePricesNewViewModel, FramePricesNewViewModel>();

            services.AddScoped<IViewModelFactory, ViewModelFactory>();
            return services;
        }
    }
}
