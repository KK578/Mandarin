using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Artists;
using Mandarin.Services.Fruity;
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
            var data = new List<ArtistDetailsModel> { TestData.Create<ArtistDetailsModel>() }.AsReadOnly();
            var artistsService = new Mock<IArtistService>();
            artistsService.Setup(x => x.GetArtistDetails()).ReturnsAsync(data);

            // Act
            var subject = new ArtistsPageViewModel(artistsService.Object);

            // Assert
            Assert.That(subject.ViewModels, Has.Count.EqualTo(1));
        }
    }
}
