using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MyReliableSite.Infrastructure.Swagger;

internal class AddApiKeyFilter : IOperationFilter
{

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (context.MethodInfo.GetCustomAttribute(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute attribute)
        {
            if (attribute.HeaderName.Contains(","))
            {
                foreach (string headerName in attribute.HeaderName.Split(","))
                {
                    var existingParam = operation.Parameters.FirstOrDefault(p =>
                p.In == ParameterLocation.Header && p.Name == headerName);

                    // remove description from [FromHeader] argument attribute
                    if (existingParam != null)
                    {
                        operation.Parameters.Remove(existingParam);
                    }

                    operation.Parameters.Add(new OpenApiParameter()
                    {
                        Name = headerName,
                        In = ParameterLocation.Header,
                        Description = attribute.Description,
                        Schema = string.IsNullOrEmpty(attribute.DefaultValue)
                    ? null
                    : new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(attribute.DefaultValue)
                    }
                    });
                }
            }
            else
            {
                var existingParam = operation.Parameters.FirstOrDefault(p =>
                p.In == ParameterLocation.Header && p.Name == attribute.HeaderName);

                // remove description from [FromHeader] argument attribute
                if (existingParam != null)
                {
                    operation.Parameters.Remove(existingParam);
                }

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = attribute.HeaderName,
                    In = ParameterLocation.Header,
                    Description = attribute.Description,
                    Schema = string.IsNullOrEmpty(attribute.DefaultValue)
                    ? null
                    : new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(attribute.DefaultValue)
                    }
                });
            }

            if (operation.Parameters.Count(m => m.Name == "admin-api-key") == 0
                && !attribute.IsClient
                && HeaderControllerConstant.AdminAPiKey.Modules.Contains(attribute.ModuleName))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "admin-api-key",
                    In = ParameterLocation.Header,
                    Description = "This attribute is only required in case of using application from admin api key. Default: S0M3Admin",
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(string.Empty)
                    }
                });
            }

            if (operation.Parameters.Count(m => m.Name == "gen-api-key") == 0
                 && HeaderControllerConstant.GenAPiKey.Modules.Contains(attribute.ModuleName))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "gen-api-key",
                    In = ParameterLocation.Header,
                    Description = "This attribute is only required in case of using application from user tokens or fetching tokens. Default: S0M3RAN0MS3CR3T",
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(string.Empty)
                    }
                });
            }

            if (operation.Parameters.Count(m => m.Name == "user-api-key") == 0
                 && HeaderControllerConstant.UserAPiKey.Modules.Contains(attribute.ModuleName))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "user-api-key",
                    In = ParameterLocation.Header,
                    Description = "This attribute is only required in case of using application from user api key.  no default",
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(string.Empty)
                    }
                });
            }

            if (operation.Parameters.Count(m => m.Name == "modulename") == 0 && attribute.IsModuleCheckReq)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "modulename",
                    In = ParameterLocation.Header,
                    Description = "Module Name i.e. " + attribute.ModuleName,
                    Schema = string.IsNullOrEmpty(attribute.ModuleName)
                    ? null
                    : new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(attribute.ModuleName)
                    }
                });
            }

            if (operation.Parameters.Count(m => m.Name == "moduleactionname") == 0 && attribute.IsModuleCheckReq)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "moduleactionname",
                    In = ParameterLocation.Header,
                    Description = "Module Action i.e. " + attribute.ModuleAction,

                    Schema = string.IsNullOrEmpty(attribute.ModuleAction)
                    ? null
                    : new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString(attribute.ModuleAction)
                    }
                });
            }
        }
    }
}
