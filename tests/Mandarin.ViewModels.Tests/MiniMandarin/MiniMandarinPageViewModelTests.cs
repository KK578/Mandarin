using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Mandarin.ViewModels.MiniMandarin;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.MiniMandarin
{
    [TestFixture]
    public class MiniMandarinPageViewModelTests
    {
        [Test]
        public async Task TextContent_ShouldBeHtmlContent()
        {
            var subject = new MiniMandarinPageViewModel(null);
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(subject.TextContent.Value));
            var link = document.Body.QuerySelector("a");
            Assert.That(link.Attributes["href"].Value, Is.EqualTo("https://instagram.com/theminimandarin_e17"));
        }

        [Test]
        public void BannerImageViewModel_ShouldPointToBanner()
        {
            var subject = new MiniMandarinPageViewModel(null);
            Assert.That(subject.BannerImageViewModel.SourceUrl.ToString(), Contains.Substring("Banner"));
            Assert.That(subject.BannerImageViewModel.Description, Contains.Substring("Bearcarons"));
        }

        [Test]
        public void MacaronImageViewModels_ShouldHave3Images()
        {
            var subject = new MiniMandarinPageViewModel(null);
            Assert.That(subject.MacaronImageViewModels, Has.Count.EqualTo(3));
        }
    }
}
