using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
            this.factory = new WebApplicationFactory<MandarinStartup>()
                .WithWebHostBuilder(c =>
                {
                    c.ConfigureTestServices(s => s.AddSingleton<IIndexPageViewModel, TestIndexPageViewModel>());
                });
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
            Assert.That(pageResponse, Contains.Substring("Simple Content"));
        }

        internal sealed class TestIndexPageViewModel : IIndexPageViewModel
        {
            public IReadOnlyList<string> Paragraphs { get; }

            public TestIndexPageViewModel()
            {
                Paragraphs = new List<string>
                {
                    "Simple Content"
                }.AsReadOnly();
            }
        }
    }
}
