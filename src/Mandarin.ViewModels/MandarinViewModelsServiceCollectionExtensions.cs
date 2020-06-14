using Mandarin.ViewModels.Artists;
using Mandarin.ViewModels.Components.Navigation;
using Mandarin.ViewModels.Contact;
using Mandarin.ViewModels.Index;
using Mandarin.ViewModels.Index.Carousel;
using Mandarin.ViewModels.Index.MandarinMap;
using Mandarin.ViewModels.Index.OpeningTimes;
using Mandarin.ViewModels.MiniMandarin;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddTransient<IMandarinHeaderViewModel, MandarinHeaderViewModel>();

            services.AddTransient<IIndexPageViewModel, IndexPageViewModel>();
            services.AddTransient<ICarouselViewModel, CarouselViewModel>();
            services.AddTransient<IMandarinMapViewModel, MandarinMapViewModel>();
            services.AddTransient<IOpeningTimesViewModel, OpeningTimesViewModel>();

            services.AddTransient<IArtistsPageViewModel, ArtistsPageViewModel>();

            services.AddTransient<IMiniMandarinPageViewModel, MiniMandarinPageViewModel>();

            services.AddTransient<IContactPageViewModel, ContactPageViewModel>();

            return services;
        }
    }
}
