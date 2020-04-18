using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
                .UseSerilog((h, l) => l.ReadFrom.Configuration(h.Configuration))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<MandarinStartup>());
    }
}
