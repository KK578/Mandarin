using Autofac;
using FluentValidation;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Client.ViewModels.DevTools;
using Mandarin.Client.ViewModels.Index;
using Mandarin.Client.ViewModels.Inventory.FramePrices;

namespace Mandarin.Client.ViewModels
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Client.ViewModels services.
    /// </summary>
    public sealed class MandarinClientViewModelsModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<IndexViewModel>().As<IIndexViewModel>().InstancePerDependency();

            builder.RegisterType<ArtistValidator>().As<IValidator<IArtistViewModel>>().InstancePerDependency();
            builder.RegisterType<ArtistsIndexViewModel>().As<IArtistsIndexViewModel>().InstancePerDependency();
            builder.RegisterType<ArtistsEditViewModel>().As<IArtistsEditViewModel>().InstancePerDependency();
            builder.RegisterType<ArtistsNewViewModel>().As<IArtistsNewViewModel>().InstancePerDependency();

            builder.RegisterType<FramePriceValidator>().As<IValidator<IFramePriceViewModel>>().InstancePerDependency();
            builder.RegisterType<FramePricesIndexViewModel>().As<IFramePricesIndexViewModel>().InstancePerDependency();
            builder.RegisterType<FramePricesEditViewModel>().As<IFramePricesEditViewModel>().InstancePerDependency();
            builder.RegisterType<FramePricesNewViewModel>().As<IFramePricesNewViewModel>().InstancePerDependency();

            builder.RegisterType<DevToolsIndexPageViewModel>().As<IDevToolsIndexPageViewModel>().InstancePerDependency();

            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().InstancePerLifetimeScope();
        }
    }
}
