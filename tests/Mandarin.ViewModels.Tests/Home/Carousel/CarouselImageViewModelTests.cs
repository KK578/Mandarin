using Bashi.Tests.Framework.Data;
using Mandarin.ViewModels.Components.Images;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Home.Carousel
{
    [TestFixture]
    public class CarouselImageViewModelTests
    {
        [Test]
        public void Constructor_GivenInvalidUrl_ShouldThrowArgumentException()
        {
            const string sourceUrl = "/oops#that#doesn't#work";
            Assert.That(() => new MandarinImageViewModel(sourceUrl, TestData.NextString()), Throws.ArgumentException);
        }

        [Test]
        public void SourceUrl_ShouldReturnValueFromConstructor()
        {
            const string sourceUrl = "https://localhost/image.png";
            var subject = new MandarinImageViewModel(sourceUrl, TestData.NextString());
            Assert.That(subject.SourceUrl.ToString(), Is.EqualTo(sourceUrl));
        }

        [Test]
        public void SourceUrl_GivenRelativeUrl_ShouldReturnValueFromConstructor()
        {
            const string sourceUrl = "/image.png";
            var subject = new MandarinImageViewModel(sourceUrl, TestData.NextString());
            Assert.That(subject.SourceUrl.ToString(), Is.EqualTo(sourceUrl));
        }
    }
}
