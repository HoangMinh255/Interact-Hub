using InteractHub.Infrastructure.Options;

namespace InteractHub.Api.Extensions;

public static class CorsExtensions
{
    public const string ReactClientPolicyName = "ReactClient";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));

        return services.AddCors(options =>
        {
            options.AddPolicy(ReactClientPolicyName, policy =>
            {
                var corsConfig = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()
                    ?? new CorsOptions();

                var origins = corsConfig.AllowedOrigins.Length > 0
                    ? corsConfig.AllowedOrigins
                    : ["http://localhost:5173", "http://localhost:3000"];

                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}