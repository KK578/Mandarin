using System;
using System.Globalization;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Mandarin.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Client
{
    /// <summary>
    /// Application entrypoint for Mandarin WebAssembly App.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entrypoint for Mandarin WebAssembly App.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> representing the application running state.</returns>
        public static Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-GB");

            // Default settings HttpClientHandler creates a new Activity anytime a request is made.
            // The resulting activity is then attached to outbound requests with DistributedTracingData set.
            // The traceparent header set at the time will then configure Elastic APM on the server side to NOT record the trace.
            // Can't find the settings to change it to enable tracing, so in the meantime, we're configuring the hidden flag
            //  so that HttpClientHandler will not attach the traceparent onto the HttpClient call.
            // Hopefully in the future, this can be replaced by some sort of Elastic APM variant to show interactions from a single page application.
            AppContext.SetSwitch("System.Net.Http.EnableActivityPropagation", false);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazorise(o => o.DelayTextOnKeyPress = true).AddBootstrapProviders().AddFontAwesomeIcons();
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            });

            builder.ConfigureContainer(new AutofacServiceProviderFactory(Program.ConfigureContainer));
            builder.Services.AddMandarinClientServices(new Uri(builder.HostEnvironment.BaseAddress));

            var host = builder.Build();
            host.Services.UseBootstrapProviders().UseFontAwesomeIcons();
            return host.RunAsync();
        }

        private static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<MandarinClientModule>();
        }
    }
}
