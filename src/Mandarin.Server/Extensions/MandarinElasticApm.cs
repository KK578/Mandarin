using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Mandarin.Server.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IApplicationBuilder"/> to register Elastic services.
    /// </summary>
    internal static class MandarinElasticApm
    {
        /// <summary>
        /// This registers Elastic APM Agent in the <paramref name="app"/>, using the provided <paramref name="configuration"/>.
        /// <br/>
        /// Due to an existing <a href="https://github.com/elastic/apm-agent-dotnet/issues/441">apm-agent-dotnet issue</a>,
        /// APM cannot handle the tracer being set up multiple times in the same application, which is done in Integration Tests.
        /// </summary>
        /// <param name="app">The application that Elastic APM agent will be registered into.</param>
        /// <param name="configuration">The application configuration to configure the Elastic APM Agent.</param>
        public static void SafeUseAllElasticApm(this IApplicationBuilder app, IConfiguration configuration)
        {
            try
            {
                app.UseAllElasticApm(configuration);
            }
            catch
            {
                // Ignored.
            }
        }
    }
}
