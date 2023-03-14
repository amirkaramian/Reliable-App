using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyReliableSite.Application.Identity.Exceptions;
using MyReliableSite.Application.Settings;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Infrastructure.Multitenancy;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MyReliableSite.Infrastructure.Identity.Extensions;

public static class IdentityExtensions
{
    internal static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<JwtSettings>(config.GetSection(nameof(JwtSettings)))
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.AddJwtAuthentication();
        return services;
    }

    internal static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services)
    {
        var jwtSettings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Key);
        _ = services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero,
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        var list = new List<string>
                        {
                            "/api/identity/verifyRecaptcha",
                            "/api/identity/confirm-email",
                            "/api/identity/confirm-phone-number",
                            "/api/identity/forgot-password",
                            "/api/identity/reset-password",
                            "/api/identity/change-password",
                            "/api/tokens/loginclientasadmin"
                        };

                        if (!context.Response.HasStarted &&
                        context.Request.Headers.TryGetValue("gen-api-key", out var extractedUserTokenApiKey)
                         && !list.Any(s => context.HttpContext.Request.Path.StartsWithSegments(s)))
                        {
                            throw new IdentityException("Authentication Failed.", statusCode: HttpStatusCode.Unauthorized);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = _ =>
                    {
                        throw new IdentityException("You are not authorized to access this resource.", statusCode: HttpStatusCode.Forbidden);
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/notifications"))
                        {
                                // Read the token out of the query string
                                context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
}