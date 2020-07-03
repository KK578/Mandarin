using System.Net;
using System.Threading.Tasks;
using Mandarin.Tests.Factory;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Mandarin.Tests.Static
{
    [TestFixture]
    public class LegacyRedirectTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;

        public LegacyRedirectTests()
        {
            this.factory = MandarinApplicationFactory.Create();
        }

        [Test]
        [TestCase("/static/logo-300.png", "/static/images/logo.png")]
        [TestCase("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf")]
        [TestCase("/the-mini-mandarin", "/macarons")]
        public async Task GetStaticPath_GivenLegacyPath_ShouldRedirect(string requestPath, string expectedRedirect)
        {
            var client = this.factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync(requestPath);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.Location.ToString(), Is.EqualTo(expectedRedirect));
        }
    }
}
