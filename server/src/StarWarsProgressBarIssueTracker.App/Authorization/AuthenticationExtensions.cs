using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace StarWarsProgressBarIssueTracker.App.Authorization;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthorization(this IServiceCollection services,
        IConfigurationSection keycloakConfiguration)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = keycloakConfiguration.GetValue<string>("Authority");
                options.Audience = keycloakConfiguration.GetValue<string>("Audience");
                options.MetadataAddress = keycloakConfiguration.GetValue<string>("MetadataAddress")!;
                options.TokenValidationParameters = new()
                {
                    ValidIssuer = keycloakConfiguration.GetValue<string>("ValidIssuer"),
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true
                };
            });
        return services;
    }
}
