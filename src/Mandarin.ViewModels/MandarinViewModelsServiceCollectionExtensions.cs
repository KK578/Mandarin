using System;
using System.IO;
using Mandarin.Configuration;
using Mandarin.ViewModels.About;
using Mandarin.ViewModels.Artists;
using Mandarin.ViewModels.Components.Navigation;
using Mandarin.ViewModels.Contact;
using Mandarin.ViewModels.Home;
using Mandarin.ViewModels.Home.Carousel;
using Mandarin.ViewModels.Home.MandarinMap;
using Mandarin.ViewModels.Home.OpeningTimes;
using Mandarin.ViewModels.Macarons;
using Markdig;
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
            services.AddSingleton(MandarinViewModelsServiceCollectionExtensions.CreateMarkdownPipeline);
            services.AddTransient<IViewModelFactory, ViewModelFactory>();

            // TODO: Singleton due to file read on construction.
            //       Should be Transient with a cache? Allows live updating of website.
            services.AddSingleton(MandarinViewModelsServiceCollectionExtensions.CreatePageContentModel);
            services.AddTransient<IMandarinHeaderViewModel, MandarinHeaderViewModel>();

            services.AddTransient<IHomePageViewModel, HomePageViewModel>();
            services.AddTransient<ICarouselViewModel, CarouselViewModel>();
            services.AddTransient<IMandarinMapViewModel, MandarinMapViewModel>();
            services.AddTransient<IOpeningTimesViewModel, OpeningTimesViewModel>();

            services.AddTransient<IArtistsPageViewModel, ArtistsPageViewModel>();

            services.AddTransient<IMacaronsPageViewModel, MacaronsPageViewModel>();

            services.AddTransient<IAboutPageViewModel, AboutPageViewModel>();

            services.AddTransient<IContactPageViewModel, ContactPageViewModel>();

            return services;
        }

        private static MarkdownPipeline CreateMarkdownPipeline(IServiceProvider provider)
        {
            return new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        private static PageContentModel CreatePageContentModel(IServiceProvider provider)
        {
            var configuration = provider.GetService<IOptions<MandarinConfiguration>>();
            using var fileStream = File.OpenRead(configuration.Value.PageContentFilePath);
            using var streamReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(streamReader);
            var token = JToken.Load(jsonReader);
            return new PageContentModel(provider.GetService<MarkdownPipeline>(), token);
        }
    }
}
