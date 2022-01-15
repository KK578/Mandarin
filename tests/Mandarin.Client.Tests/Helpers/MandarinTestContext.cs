using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ReactiveUI;

namespace Mandarin.Client.Tests.Helpers
{
    public abstract class MandarinTestContext<TView, TViewModel> : TestContext
        where TView : IComponent
        where TViewModel : class, IReactiveObject
    {
        protected MandarinTestContext()
        {
            this.Services.AddSingleton(this.ViewModel.Object);
            this.SetupBlazorise();
        }

        protected Mock<TViewModel> ViewModel { get; } = new();

        protected IRenderedComponent<TView> Subject => this.RenderComponent<TView>();

        private void SetupBlazorise()
        {
            this.Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();

            var blazoriseModule = this.JSInterop.SetupModule(i => i.Arguments[0] is string requestedModule && requestedModule.StartsWith("./_content/Blazorise/"));
            blazoriseModule.SetupVoid("initialize", _ => true);
            blazoriseModule.Setup<string>("getUserAgent");
        }
    }
}
