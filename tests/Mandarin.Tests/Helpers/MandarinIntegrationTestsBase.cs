using System.Threading.Tasks;
using Mandarin.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mandarin.Tests.Helpers
{
    public abstract class MandarinIntegrationTestsBase
    {
        private IServiceScope scope;

        protected MandarinIntegrationTestsBase()
        {
            this.Factory = new MandarinApplicationFactory()
                .WithWebHostBuilder(b => b.UseTestServer().ConfigureTestServices(this.ConfigureTestServices));
        }

        protected WebApplicationFactory<MandarinStartup> Factory { get; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // NOTE: Database Migration is handled by application startup.
            await MandarinIntegrationTestsBase.WriteLogAsync("<OneTimeSetUp>");

            this.scope = this.Factory.Services.CreateScope();

            await MandarinIntegrationTestsBase.WriteLogAsync("</OneTimeSetUp>");
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await MandarinIntegrationTestsBase.WriteLogAsync("<OneTimeTearDown>");

            var mandarinDbContext = this.scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            var migrator = mandarinDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
            this.scope.Dispose();

            await MandarinIntegrationTestsBase.WriteLogAsync("</OneTimeTearDown>");
        }

        [SetUp]
        public async Task SetUp()
        {
            await MandarinIntegrationTestsBase.WriteLogAsync("<SetUp>");

            this.scope = this.Factory.Services.CreateScope();
            var mandarinDbContext = this.scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.SeedTestDataAsync();

            await MandarinIntegrationTestsBase.WriteLogAsync("</SetUp>");
        }

        [TearDown]
        public async Task TearDown()
        {
            await MandarinIntegrationTestsBase.WriteLogAsync("<TearDown>");

            var mandarinDbContext = this.scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.CleanupTestDataAsync();

            await MandarinIntegrationTestsBase.WriteLogAsync("</TearDown>");
        }

        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
        }

        private static async Task WriteLogAsync(string message)
        {
            await TestContext.Progress.WriteLineAsync(new string('#', message.Length + 8));
            await TestContext.Progress.WriteLineAsync($"### {message} ###");
            await TestContext.Progress.WriteLineAsync(new string('#', message.Length + 8));
        }
    }
}
