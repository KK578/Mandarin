using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Mandarin.Configuration;
using Mandarin.Extensions;
using Mandarin.Services;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mandarin
{
    /// <summary>
    /// ASP.NET Core services configuration for Mandarin public-facing application.
    /// </summary>
    public class MandarinStartup
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinStartup"/> class.
        /// </summary>
        /// <param name="configuration">The master Application configuration.</param>
        public MandarinStartup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Configures the provided service collection with all the dependencies required for Mandarin.
        /// <br />
        /// Currently this includes:
        /// <list type="bullet">
        /// <item><term>Razor Pages</term></item>
        /// <item><term>Server Side Blazor</term></item>
        /// <item><term>Authentication</term></item>
        /// <item><term>Application Services</term></item>
        /// <item><term>View Models</term></item>
        /// </list>
        ///
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit <a href="https://go.microsoft.com/fwlink/?LinkID=398940">https://go.microsoft.com/fwlink/?LinkID=398940</a>.
        /// </remarks>
        /// </summary>
        /// <param name="services">Service collection to be modified.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();
            services.AddHttpContextAccessor();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<MandarinConfiguration>(this.configuration.GetSection("Mandarin"));
            services.AddMandarinAuthentication(this.configuration);
            services.AddMandarinServices(this.configuration);
            services.AddMandarinViewModels();
        }

        /// <summary>
        /// Configures the HTTP Pipeline for Mandarin.
        ///
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        /// </summary>
        /// <param name="app">The application to be configured.</param>
        public void Configure(IApplicationBuilder app)
        {
            app.SafeUseAllElasticApm(this.configuration);

            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.AddLegacyRedirect("/static/logo-300.png", "/static/images/logo.png");
            app.AddLegacyRedirect("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf");
            app.AddLegacyRedirect("/the-mini-mandarin", "/macarons");

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices.UseBootstrapProviders().UseBootstrapProviders();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
