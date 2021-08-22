using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mandarin.Database;
using Mandarin.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Mandarin
{
    /// <summary>
    /// Application entry point for Mandarin public-facing application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// Main entry point for starting the Mandarin public-facing web application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");
                Program.CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(Program.ConfigureSerilog)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(Program.ConfigureContainer)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<MandarinStartup>());

        private static void ConfigureSerilog(HostBuilderContext h, LoggerConfiguration l)
        {
            l.ReadFrom.Configuration(h.Configuration);
        }

        private static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<MandarinDatabaseModule>();
            builder.RegisterModule<MandarinInterfacesModule>();
            builder.RegisterModule<MandarinServicesModule>();
        }
    }
}
