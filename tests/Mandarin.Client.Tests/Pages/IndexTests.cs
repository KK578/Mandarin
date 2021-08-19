using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using FluentAssertions;
using Mandarin.Client.ViewModels.Index;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Index = Mandarin.Client.Pages.Index;

namespace Mandarin.Client.Tests.Pages
{
    public class IndexTests : TestContext
    {
        [Fact]
        public void ShouldContainTheLogo()
        {
            var viewModel = new Mock<IIndexViewModel>();
            this.Services.AddSingleton(viewModel.Object);
            this.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            var cut = this.RenderComponent<Index>();

            var image = cut.FindComponent<FigureImage>();
            image.Instance.Source.Should().Be("/static/images/logo.png");
        }
    }
}
