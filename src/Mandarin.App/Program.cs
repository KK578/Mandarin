using System.Threading.Tasks;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Mandarin.App.Client;
using Mandarin.App.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Mandarin.App
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
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazorise(o => o.DelayTextOnKeyPress = true).AddBootstrapProviders().AddFontAwesomeIcons();
            builder.AddMandarinClient();
            builder.AddMandarinAuthentication();

            var host = builder.Build();
            host.Services.UseBootstrapProviders().UseFontAwesomeIcons();
            return host.RunAsync();
        }
    }
}
