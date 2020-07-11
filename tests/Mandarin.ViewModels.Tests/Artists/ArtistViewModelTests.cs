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
            var subject = new ArtistViewModel(new Stockist { Details = new StockistDetail() });

            // Assert
            Assert.That(subject.ShortName, Is.Null);
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
            var model = new Stockist
            {
                StockistCode = null,
                ShortDisplayName = name,
                FullDisplayName = name,
                Description = description,
            };
            var subject = new ArtistViewModel(model);

            // Assert
            Assert.That(subject.ShortName, Is.EqualTo(name));
            Assert.That(subject.Description, Is.EqualTo(description));
        }

        [Test]
        public void Builder_GivenUri_CreatesValueCorrectly()
        {
            // Act
            var model = new Stockist
            {
                StockistCode = null,
                FullDisplayName = null,
                Description = null,
                Details = new StockistDetail
                {
                    EmailAddress = null,
                    BannerImageUrl = "https://localhost/image",
                    TwitterHandle = "MyTwitterHandle",
                    InstagramHandle = "MyInstagramHandle",
                    FacebookHandle = "MyFacebookHandle",
                    TumblrHandle = "MyTumblrHandle",
                    WebsiteUrl = "https://localhost/website",
                },
            };
            var subject = new ArtistViewModel(model);

            // Assert
            Assert.That(subject.TwitterUrl.ToString(), Is.EqualTo("https://twitter.com/MyTwitterHandle"));
            Assert.That(subject.FacebookUrl.ToString(), Is.EqualTo("https://facebook.com/MyFacebookHandle"));
            Assert.That(subject.ImageUrl.ToString(), Is.EqualTo("https://localhost/image"));
            Assert.That(subject.InstagramUrl.ToString(), Is.EqualTo("https://instagram.com/MyInstagramHandle"));
            Assert.That(subject.TumblrUrl.ToString(), Is.EqualTo("https://MyTumblrHandle.tumblr.com/").IgnoreCase);
            Assert.That(subject.WebsiteUrl.ToString(), Is.EqualTo("https://localhost/website"));
        }
    }
}
