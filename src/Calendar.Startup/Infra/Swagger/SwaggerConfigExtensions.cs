using Asp.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Calendar.Startup.Infra.Swagger
{
    internal static class SwaggerConfigExtensions
    {

        internal static IServiceCollection addApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.ReportApiVersions = true;
            });

            return services;
        }

        internal static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Calendar Api",
                    Description = "Web api to manage / fetch calendar events",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Edward MAIRE",
                        Email = "edward.maire.1988@gmail.com"
                    }
                });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
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

                options.OperationFilter<RemoveDefaultResponseOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                var libraryXmlFile = "Calendar.Api.xml";
                var libraryXmlPath = Path.Combine(AppContext.BaseDirectory, libraryXmlFile);
                options.IncludeXmlComments(libraryXmlPath);
            });

            return services;
        }
    }
}
