using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Macarons;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Macarons
{
    [TestFixture]
    public class MacaronsPageViewModelTests
    {
        [Test]
        public void TextContent_ShouldBeHtmlContent()
        {
            var data = new
            {
                Macarons = new
                {
                    MainText = "This *is* markdown!.",
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MacaronsPageViewModel(pageContentModel);
            Assert.That(subject.TextContent.Value, Is.EqualTo("<p>This <em>is</em> markdown!.</p>\n"));
        }

        [Test]
        public void BannerImageViewModel_ShouldPointToBanner()
        {
            var data = new
            {
                Macarons = new
                {
                    MainText = TestData.NextString(),
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MacaronsPageViewModel(pageContentModel);
            Assert.That(subject.BannerImageViewModel.SourceUrl.ToString(), Is.EqualTo(data.Macarons.BannerImage.Url));
            Assert.That(subject.BannerImageViewModel.Description, Is.EqualTo(data.Macarons.BannerImage.Description));
        }

        [Test]
        public void MacaronImageViewModels_ShouldHaveMatchingNumberOfImages()
        {
            var data = new
            {
                Macarons = new
                {
                    MainText = TestData.NextString(),
                    BannerImage = TestData.Create<ImageUrlModel>(),
                    MacaronImages = TestData.Create<List<ImageUrlModel>>(),
                },
            };

            var pageContentModel = new PageContentModel(null, JToken.FromObject(data));
            var subject = new MacaronsPageViewModel(pageContentModel);
            Assert.That(subject.MacaronImageViewModels, Has.Count.EqualTo(data.Macarons.MacaronImages.Count));
        }
    }
}
