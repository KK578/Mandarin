using Mandarin.ViewModels.Artists;
using Moq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Artists
{
    [TestFixture]
    public class ArtistsPageViewModelTests
    {
        [Test]
        public void ViewModels_Given1ViewModel_ShouldHaveCountOf1()
        {
            // Arrange
            var data = Mock.Of<IArtistViewModel>();

            // Act
            var subject = new ArtistsPageViewModel(new [] { data });

            // Assert
            Assert.That(subject.ViewModels, Has.Count.EqualTo(1));
        }
    }
}
