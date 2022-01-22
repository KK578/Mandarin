using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mandarin.Extensions;

/// <summary>
/// Extensions to <see cref="IApplicationBuilder"/> to register health checks.
/// </summary>
internal static class MandarinHealthChecks
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
    };

    /// <summary>
    /// Adds services for providing health check status.
    /// </summary>
    /// <param name="services">Service container to add registrations to.</param>
    /// <param name="configuration">Application configuration for configuring services.</param>
    /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which health checks can be registered.</returns>
    public static IHealthChecksBuilder AddMandarinHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddHealthChecks()
                       .AddSendGrid(configuration.GetValue<string>("SendGrid:ApiKey"), "SendGrid")
                       .AddNpgSql(configuration.GetConnectionString("MandarinConnection"));
    }

    /// <summary>
    /// Adds a middleware on "/healthz" that provides health check status.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    public static void UseMandarinHealthChecks(this IApplicationBuilder app)
    {
        var options = new HealthCheckOptions { ResponseWriter = MandarinHealthChecks.WriteHealthCheckResponse };
        app.UseHealthChecks("/healthz", options);
    }

    private static Task WriteHealthCheckResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.WriteAsJsonAsync(healthReport, MandarinHealthChecks.JsonSerializerOptions);
        return Task.CompletedTask;
    }
}
