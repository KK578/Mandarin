using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Events;

namespace Mandarin.Tests.Factory
{
    public static class MandarinApplicationFactory
    {
        public static WebApplicationFactory<MandarinStartup> Create()
        {
            var factory = new WebApplicationFactory<MandarinStartup>();
            return factory.WithWebHostBuilder(b => b.UseSerilog(MandarinApplicationFactory.ConfigureSerilog));
        }

        private static void ConfigureSerilog(WebHostBuilderContext b, LoggerConfiguration c)
        {
            c.MinimumLevel.Verbose()
             .MinimumLevel.Override("Elastic.Apm", LogEventLevel.Error)
             .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.ffff} {Level:u3} {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
    }
}
