using Elastic.Apm.NetCoreAll;
using Mandarin.Database;
using Mandarin.Extensions;
using Mandarin.Grpc;
using Mandarin.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
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

            services.AddMandarinAuthentication(this.configuration);
            services.AddMandarinAuthorization();
            services.AddMandarinHangfire(this.configuration);

            services.AddHttpClient<SendGridClient>();
        }

        /// <summary>
        /// Configures the HTTP Pipeline for Mandarin.
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        /// </summary>
        /// <param name="app">The application to be configured.</param>
        /// <param name="env">The application host environment.</param>
        /// <param name="mandarinDbContext">The application database context.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MandarinDbContext mandarinDbContext)
        {
            mandarinDbContext.RunMigrations();

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
            app.AddLegacyRedirect("/static/logo-300.png", "/static/images/logo.png");
            app.AddLegacyRedirect("/static/Century-Schoolbook-Std-Regular.otf", "/static/fonts/Century-Schoolbook-Std-Regular.otf");

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAllElasticApm(this.configuration);
            app.UseSerilogRequestLogging();

            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseMandarinHangfire(this.configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<CommissionsGrpcService>();
                endpoints.MapGrpcService<EmailGrpcService>();
                endpoints.MapGrpcService<FramePricesGrpcService>();
                endpoints.MapGrpcService<ProductsGrpcService>();
                endpoints.MapGrpcService<StockistsGrpcService>();
                endpoints.MapGrpcService<TransactionsGrpcService>();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
