using System.Reactive.Linq;
using AutoFixture;
using Mandarin.Models.Artists;
using Mandarin.Services;
using Mandarin.Tests.Data;
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
            var data = MandarinFixture.Instance.Create<Stockist>();
            var artistsService = new Mock<IArtistService>();
            artistsService.Setup(x => x.GetArtistsForDisplayAsync()).Returns(Observable.Return(data));

            // Act
            var subject = new ArtistsPageViewModel(artistsService.Object);

            // Assert
            Assert.That(subject.ViewModels, Has.Count.EqualTo(1));
        }
    }
}
