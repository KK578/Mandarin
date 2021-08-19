using System;
using Blazorise;
using Bunit;
using FluentAssertions;
using Mandarin.Client.Tests.Helpers;
using Mandarin.Client.ViewModels.Index;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Index = Mandarin.Client.Pages.Index;

namespace Mandarin.Client.Tests.Pages
{
    public sealed class IndexTests : MandarinTestContext
    {
        private readonly Mock<IIndexViewModel> indexViewModel = new();

        public IndexTests()
        {
            this.Services.AddSingleton(this.indexViewModel.Object);
        }

        private IRenderedComponent<Index> Subject => this.RenderComponent<Index>();

        [Fact]
        public void ShouldContainTheLogo()
        {
            var image = this.Subject.FindComponent<FigureImage>();
            image.Instance.Source.Should().Be("/static/images/logo.png");
        }

        [Fact]
        public void ShouldContainTheCompanyName()
        {
            var title = this.Subject.FindComponent<JumbotronTitle>();
            title.Markup.Should().Contain("The Little Mandarin");
        }

        [Fact]
        public void ShouldDisplayTheApplicationVersion()
        {
            this.indexViewModel.Setup(x => x.Version).Returns(new Version(1, 2, 3, 4));
            this.Subject.Markup.Should().Contain("version 1.2.3.4");
        }

        [Fact]
        public void ShouldContainTheIconToDisplay()
        {
            this.indexViewModel.Setup(x => x.Icon).Returns(IconName.Brush);
            var icon = this.Subject.FindComponent<Icon>();
            icon.Instance.Name.Should().Be(IconName.Brush);
        }
    }
}
