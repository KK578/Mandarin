using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture]
    public class IndexPageTests
    {
        private WebApplicationFactory<MandarinStartup> factory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.factory = new WebApplicationFactory<MandarinStartup>();
        }

        [Test]
        public async Task Get()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing);
            Assert.That(response.Content.Headers.ContentType.MediaType, Contains.Substring("text/html"));

            var pageResponse = await response.Content.ReadAsStringAsync();
            Assert.That(pageResponse, Contains.Substring("We hope to see you soon!"));
        }
    }
}
