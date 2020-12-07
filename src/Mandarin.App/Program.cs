using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
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

            builder.AddMandarinWasm();

            var host = builder.Build();
            host.Services.UseBootstrapProviders().UseFontAwesomeIcons();
            return host.RunAsync();
        }
    }
}
