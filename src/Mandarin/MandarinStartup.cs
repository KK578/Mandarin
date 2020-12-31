using AutoMapper;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Mandarin.Configuration;
using Mandarin.Converters;
using Mandarin.Database;
using Mandarin.Extensions;
using Mandarin.Grpc;
using Mandarin.Services;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

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
            services.AddBlazorise(o => o.DelayTextOnKeyPress = true).AddBootstrapProviders().AddFontAwesomeIcons();
            services.AddGrpc();
            services.AddHttpContextAccessor();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<MandarinConfiguration>(this.configuration.GetSection("Mandarin"));
            services.AddMandarinAuthentication(this.configuration);
            services.AddMandarinDatabase(this.configuration);
            services.AddAutoMapper(options => { options.AddProfile<MandarinMapperProfile>(); });
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
        /// <param name="env">The application host environment.</param>
        /// <param name="mandarinDbContext">The application database context.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MandarinDbContext mandarinDbContext)
        {
            mandarinDbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.SafeUseAllElasticApm(this.configuration);
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.AddLegacyRedirect("/static/logo-300.png", "/static/images/logo.png");
            app.AddLegacyRedirect("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf");

            app.UseRouting();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices.UseBootstrapProviders().UseBootstrapProviders();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGrpcService<CommissionsGrpcService>();
                endpoints.MapGrpcService<EmailGrpcService>();
                endpoints.MapGrpcService<StockistsGrpcService>();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
