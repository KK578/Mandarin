using System;
using System.IO;
using Mandarin.Configuration;
using Mandarin.ViewModels.Artists;
using Mandarin.ViewModels.Components.Navigation;
using Mandarin.ViewModels.Contact;
using Mandarin.ViewModels.Index;
using Mandarin.ViewModels.Index.Carousel;
using Mandarin.ViewModels.Index.MandarinMap;
using Mandarin.ViewModels.Index.OpeningTimes;
using Mandarin.ViewModels.MiniMandarin;
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

            services.AddTransient(CreatePageContentModel);
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
