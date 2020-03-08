using Mandarin.ViewModels.Index.Carousel;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.Carousel
{
    [TestFixture]
    public class CarouselViewModelTests
    {
        [Test]
        public void Images_ShouldContain4Images()
        {
            var subject = new CarouselViewModel();
            Assert.That(subject.Images, Has.Count.EqualTo(4));
        }
    }
}
