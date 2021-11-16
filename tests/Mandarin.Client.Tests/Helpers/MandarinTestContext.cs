using Blazorise;
using Blazorise.Bootstrap;
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
            this.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            this.JSInterop.Setup<bool>("blazorise.button.initialize", _ => true);
            this.JSInterop.Setup<bool>("blazorise.textEdit.initialize", _ => true);
        }
    }
}
