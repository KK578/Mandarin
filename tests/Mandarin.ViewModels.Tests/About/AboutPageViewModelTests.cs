using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.About;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.About
{
    [TestFixture]
    public class AboutPageViewModelTests
    {
        [Test]
        public void AllPropertiesShouldReadDataFromModel()
        {
            var pageContentModel = AboutPageViewModelTests.CreatePageContentModel();
            var subject = new AboutPageViewModel(pageContentModel);

            Assert.That(subject.ImageViewModel, Is.Not.Null);
            Assert.That(subject.MainContent, Is.Not.Null);
            Assert.That(subject.GiftCardContent, Is.Not.Null);
            Assert.That(subject.GiftCardImageViewModel, Is.Not.Null);
        }

        [Test]
        public void MainContent_IsAFormattedMarkdownString()
        {
            var pageContentModel = AboutPageViewModelTests.CreatePageContentModel();
            var subject = new AboutPageViewModel(pageContentModel);

            Assert.That(subject.MainContent.Value, Is.EqualTo("<p>This <em>is</em> markdown!.</p>\n"));
        }

        private static PageContentModel CreatePageContentModel()
        {
            var data = new
            {
                About = new
                {
                    Image = TestData.Create<ImageUrlModel>(),
                    MainText = "This *is* markdown!.",
                    GiftCards = new
                    {
                        Text = TestData.NextString(),
                        AnimationImage = TestData.Create<ImageUrlModel>(),
                    },
                },
            };

            return new PageContentModel(null, JToken.FromObject(data));
        }
    }
}
