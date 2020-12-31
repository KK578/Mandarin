using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Converters;
using Mandarin.Database;
using Mandarin.Tests.Data;
using Mandarin.Tests.Factory;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mandarin.Client.Services.Tests
{
    public abstract class GrpcServiceTestsBase
    {
        private readonly ServiceProvider serviceProvider;
        private readonly WebApplicationFactory<MandarinStartup> factory;

        private IServiceScope scope;

        protected GrpcServiceTestsBase()
        {
            // TODO: Merge with rest of Mandarin TestBase?
            //       Alternatively, those tests will most likely change to move all to Client.
            this.factory = MandarinApplicationFactory.Create().WithWebHostBuilder(b => b.UseTestServer());
            var server = this.factory.Server;
            var handler = server.CreateHandler();

            var services = new ServiceCollection();
            services.AddMandarinClientServices(server.BaseAddress, () => handler);
            services.AddSingleton<IAccessTokenProvider, TestAuthAccessTokenProvider>();
            services.AddAutoMapper(options => { options.AddProfile<MandarinMapperProfile>(); });
            this.serviceProvider = services.BuildServiceProvider();
        }

        [SetUp]
        public async Task SetUp()
        {
            this.scope = this.factory.Services.CreateScope();
            var mandarinDbContext = this.scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            await mandarinDbContext.Database.MigrateAsync();
            await WellKnownTestData.SeedDatabaseAsync(mandarinDbContext);
            await mandarinDbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            var mandarinDbContext = this.scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            var migrator = mandarinDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
            this.scope.Dispose();
        }

        protected T Resolve<T>() => this.serviceProvider.GetRequiredService<T>();
    }
}
