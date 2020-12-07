using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Mandarin.Server.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IApplicationBuilder"/> to register Swagger services.
    /// </summary>
    internal static class MandarinSwaggerExtensions
    {
        /// <summary>
        /// Adds the Mandarin API Swagger document.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinSwagger(this IServiceCollection services)
        {
            return services.AddOpenApiDocument(document =>
            {
                document.Title = "MandarinApi";
                document.Description = "Mandarin's Internal Admin API";
                document.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                document.DocumentName = "Mandarin";

                document.AddSecurity("JWT",
                                     Enumerable.Empty<string>(),
                                     new OpenApiSecurityScheme
                                     {
                                         Type = OpenApiSecuritySchemeType.ApiKey,
                                         Name = "Authorization",
                                         In = OpenApiSecurityApiKeyLocation.Header,
                                         Description = "Bearer {JWT_TOKEN}",
                                     });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }
    }
}
