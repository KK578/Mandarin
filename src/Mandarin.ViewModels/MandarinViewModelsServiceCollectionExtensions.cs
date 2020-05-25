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
    public static class MandarinViewModelsServiceCollectionExtensions
    {
        public static void AddMandarinViewModels(this IServiceCollection services)
        {
            services.AddTransient<IMandarinHeaderViewModel, MandarinHeaderViewModel>();

            services.AddTransient<IIndexPageViewModel, IndexPageViewModel>();
            services.AddTransient<ICarouselViewModel, CarouselViewModel>();
            services.AddTransient<IMandarinMapViewModel, MandarinMapViewModel>();
            services.AddTransient<IOpeningTimesViewModel, OpeningTimesViewModel>();

            services.AddTransient<IArtistsPageViewModel, ArtistsPageViewModel>();

            services.AddTransient<IMiniMandarinPageViewModel, MiniMandarinPageViewModel>();

            services.AddTransient<IContactPageViewModel, ContactPageViewModel>();
        }
    }
}
