using Mandarin.ViewModels.Index.MandarinMap;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.MandarinMap
{
    [TestFixture]
    public class MandarinMapViewModelTests
    {
        [Test]
        public void MapUri_ShouldFormGoogleMapEmbedUri()
        {
            var subject = new MandarinMapViewModel();
            Assert.That(subject.MapUri.ToString(), Contains.Substring("https://www.google.com/maps/embed"));
        }
    }
}
