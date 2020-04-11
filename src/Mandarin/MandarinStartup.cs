using Mandarin.Elastic;
using Mandarin.Services.Email;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        /// <item><term>Http Client</term></item>
        /// <item><term>Server Side Blazor</term></item>
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

            services.AddMandarinViewModels();
            services.AddSendGridClient(this.configuration);
            services.AddSingleton<IEmailService, SendGridEmailService>();
            services.Decorate<IEmailService, LoggingEmailServiceDecorator>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

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
