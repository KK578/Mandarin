using System.Net;
using System.Threading.Tasks;
using Mandarin.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Mandarin.Tests.Static
{
    [TestFixture]
    public class LegacyRedirectTests : MandarinIntegrationTestsBase
    {
        [Test]
        [TestCase("/static/logo-300.png", "/static/images/logo.png")]
        [TestCase("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf")]
        public async Task GetStaticPath_GivenLegacyPath_ShouldRedirect(string requestPath, string expectedRedirect)
        {
            var client = this.Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync(requestPath);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.Location.ToString(), Is.EqualTo(expectedRedirect));
        }
    }
}
