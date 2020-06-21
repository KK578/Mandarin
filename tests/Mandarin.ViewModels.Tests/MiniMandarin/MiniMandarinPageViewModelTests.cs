using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.MiniMandarin;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.MiniMandarin
{
    [TestFixture]
    public class MiniMandarinPageViewModelTests
    {
        [Test]
        public void TextContent_ShouldBeHtmlContent()
        {
            var data = new
            {
                MiniMandarin = new
                {
                    MainText = "This *is* markdown!.",
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MiniMandarinPageViewModel(pageContentModel);
            Assert.That(subject.TextContent.Value, Is.EqualTo("<p>This <em>is</em> markdown!.</p>\n"));
        }

        [Test]
        public void BannerImageViewModel_ShouldPointToBanner()
        {
            var data = new
            {
                MiniMandarin = new
                {
                    MainText = TestData.NextString(),
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MiniMandarinPageViewModel(pageContentModel);
            Assert.That(subject.BannerImageViewModel.SourceUrl.ToString(), Is.EqualTo(data.MiniMandarin.BannerImage.Url));
            Assert.That(subject.BannerImageViewModel.Description, Is.EqualTo(data.MiniMandarin.BannerImage.Description));
        }

        [Test]
        public void MacaronImageViewModels_ShouldHaveMatchingNumberOfImages()
        {
            var data = new
            {
                MiniMandarin = new
                {
                    MainText = TestData.NextString(),
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MiniMandarinPageViewModel(pageContentModel);
            Assert.That(subject.MacaronImageViewModels, Has.Count.EqualTo(data.MiniMandarin.MacaronImages.Count));
        }
    }
}
