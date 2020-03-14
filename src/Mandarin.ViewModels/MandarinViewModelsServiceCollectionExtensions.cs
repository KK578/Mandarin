using Mandarin.ViewModels.Artists;
using Mandarin.ViewModels.Components.Navigation;
using Mandarin.ViewModels.Index;
using Mandarin.ViewModels.Index.Carousel;
using Mandarin.ViewModels.Index.MandarinMap;
using Mandarin.ViewModels.Index.OpeningTimes;
using Mandarin.ViewModels.MiniMandarin;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.ViewModels
{
    public static class MandarinViewModelsServiceCollectionExtensions
    {
        public static void AddMandarinViewModels(this IServiceCollection services)
        {
            services.AddSingleton<IMandarinHeaderViewModel, MandarinHeaderViewModel>();

            services.AddSingleton<IIndexPageViewModel, IndexPageViewModel>();
            services.AddSingleton<ICarouselViewModel, CarouselViewModel>();
            services.AddSingleton<IMandarinMapViewModel, MandarinMapViewModel>();
            services.AddSingleton<IOpeningTimesViewModel, OpeningTimesViewModel>();

            services.AddSingleton<IArtistsPageViewModel, ArtistsPageViewModel>();

            services.AddSingleton<IMiniMandarinPageViewModel, MiniMandarinPageViewModel>();
        }
    }
}
