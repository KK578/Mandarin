using Bashi.Tests.Framework.Data;
using Mandarin.Models.Artists;
using Mandarin.ViewModels.Artists;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Artists
{
    [TestFixture]
    public class ArtistViewModelTests
    {
        [Test]
        public void Builder_GivenNoArguments_CreatesEmptyObject()
        {
            // Act
            var subject = new ArtistViewModel(new ArtistDetailsModel());

            // Assert
            Assert.That(subject.Name, Is.Null);
            Assert.That(subject.Description, Is.Null);
            Assert.That(subject.FacebookUrl, Is.Null);
            Assert.That(subject.ImageUrl, Is.Null);
            Assert.That(subject.InstagramUrl, Is.Null);
            Assert.That(subject.TumblrUrl, Is.Null);
            Assert.That(subject.TwitterUrl, Is.Null);
            Assert.That(subject.WebsiteUrl, Is.Null);
        }

        [Test]
        public void Builder_GivenNameAndDescription_CreatesValueCorrectly()
        {
            // Arrange
            var name = TestData.NextString();
            var description = TestData.WellKnownString;

            // Act
            var model = new ArtistDetailsModel
            {
                Name = name,
                Description = description,
            };
            var subject = new ArtistViewModel(model);

            // Assert
            Assert.That(subject.Name, Is.EqualTo(name));
            Assert.That(subject.Description, Is.EqualTo(description));
        }

        [Test]
        public void Builder_GivenUri_CreatesValueCorrectly()
        {
            // Act
            var model = new ArtistDetailsModel
            {
                Twitter = "https://localhost/twitter",
                Facebook = "https://localhost/facebook",
                Image = "https://localhost/image",
                Instagram = "https://localhost/instagram",
                Tumblr = "https://localhost/tumblr",
                Website = "https://localhost/website",
            };
            var subject = new ArtistViewModel(model);

            // Assert
            Assert.That(subject.TwitterUrl.ToString(), Contains.Substring("twitter"));
            Assert.That(subject.FacebookUrl.ToString(), Contains.Substring("facebook"));
            Assert.That(subject.ImageUrl.ToString(), Contains.Substring("image"));
            Assert.That(subject.InstagramUrl.ToString(), Contains.Substring("instagram"));
            Assert.That(subject.TumblrUrl.ToString(), Contains.Substring("tumblr"));
            Assert.That(subject.WebsiteUrl.ToString(), Contains.Substring("website"));
        }
    }
}
