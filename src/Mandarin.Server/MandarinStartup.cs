using AutoMapper;
using Mandarin.Configuration;
using Mandarin.Converters;
using Mandarin.Database;
using Mandarin.Server.Extensions;
using Mandarin.Server.Services;
using Mandarin.Services;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mandarin.Server
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
        /// <item><term>Controllers</term></item>
        /// <item><term>Authentication</term></item>
        /// <item><term>Application Services</term></item>
        /// <item><term>Swagger</term></item>
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
            services.AddGrpc();
            services.AddHttpContextAccessor();

            services.Configure<MandarinConfiguration>(this.configuration.GetSection("Mandarin"));
            services.AddMandarinAuthentication(this.configuration);
            services.AddMandarinDatabase(this.configuration);
            services.AddMandarinServices(this.configuration);
            services.AddMandarinViewModels();

            services.AddAutoMapper(options =>
            {
                options.AddProfile<MandarinMapperProfile>();
            });
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
            app.SafeUseAllElasticApm(this.configuration);
            mandarinDbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StockistsGrpcService>();
                endpoints.MapGrpcService<CommissionsGrpcService>();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
