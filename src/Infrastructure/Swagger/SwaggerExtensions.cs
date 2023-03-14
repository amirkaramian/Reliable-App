using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MyReliableSite.Application.Settings;
using MyReliableSite.Infrastructure.Multitenancy;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MyReliableSite.Infrastructure.Swagger;

public static class SwaggerExtensions
{
    internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        if (config.GetValue<bool>("SwaggerSettings:Enable"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DefaultModelsExpandDepth(-1);
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
                options.DocExpansion(DocExpansion.None);
            });
        }

        return app;
    }

    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string webPortalName)
    {
        var settings = services.GetOptions<SwaggerSettings>(nameof(SwaggerSettings));
        if (settings.Enable)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyReliableSite - " + webPortalName,
                    Version = "v1",
                    Description = ".NET 6.0 WebApi built with Multitenancy Support for My reliable site.",
                    Contact = new OpenApiContact
                    {
                        Name = "Mind2Matter",
                        Email = "no-reply@myreliablesite.com",
                        Url = new Uri("https://Mind2Matter.com"),
                    }
                });
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        string xmlFile = $"{assembly.GetName().Name}.xml";
                        string xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            options.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });

                options.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true,
                    Pattern = @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$",
                    Example = new OpenApiString("02:00:00")
                });

                options.SchemaFilter<EnumSchemaFilter>();

                options.EnableAnnotations();
                options.UseInlineDefinitionsForEnums();

                // options.OperationFilter<AddTenantIdFilter>();
                options.OperationFilter<AddApiKeyFilter>();
            });
        }

        return services;
    }
}

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            model.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => model.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, name))} = {name}")));
        }
    }
}
