using System.Reflection;
using InteractHub.Application.Common;
using Microsoft.OpenApi.Models;

namespace InteractHub.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "InteractHub API",
                Version = "v1",
                Description = "InteractHub backend API for social media application"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter only the token value.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
            if (File.Exists(apiXmlPath))
            {
                options.IncludeXmlComments(apiXmlPath);
            }

            var appXmlPath = Path.Combine(AppContext.BaseDirectory, "InteractHub.Application.xml");
            if (File.Exists(appXmlPath))
            {
                options.IncludeXmlComments(appXmlPath);
            }

            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        return services;
    }
}