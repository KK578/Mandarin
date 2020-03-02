using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Index.Carousel;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.Carousel
{
    [TestFixture]
    public class CarouselImageViewModelTests
    {
        [Test]
        public void SourceUrl_ShouldReturnValueFromConstructor()
        {
            const string sourceUrl = "https://localhost/image.png";
            var subject = new CarouselImageViewModel(sourceUrl, TestData.NextString());
            Assert.That(subject.SourceUrl.ToString(), Is.EqualTo(sourceUrl));
        }
    }
}
