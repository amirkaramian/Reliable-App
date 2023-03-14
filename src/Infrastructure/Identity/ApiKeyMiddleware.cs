using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Infrastructure.ManageModules;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using Newtonsoft.Json;
using System.Net;
using System.Security.Principal;

namespace MyReliableSite.Infrastructure.Identity;

public class ApiKeyMiddleware : IMiddleware
{
    private readonly IAPIKeyPairService _service;
    private readonly IUserService _userService;
    private readonly ISettingService _settingsService;
    private readonly IConfiguration _config;

    private readonly IpRateLimitOptions _options;
    private readonly IIpPolicyStore _ipPolicyStore;
    private readonly ClientRateLimitOptions _optionsClient;
    private readonly IClientPolicyStore _clientPolicyStore;
    public ApiKeyMiddleware(
        IConfiguration config,
        IAPIKeyPairService service,
        IUserService userService,
        ISettingService settingsService,
        IOptions<IpRateLimitOptions> optionsAccessor,
        IIpPolicyStore ipPolicyStore,
        IOptions<ClientRateLimitOptions> optionsAccessorClient,
        IClientPolicyStore clientPolicyStore)
    {
        _service = service;
        _config = config;

        _options = optionsAccessor.Value;
        _ipPolicyStore = ipPolicyStore;
        _optionsClient = optionsAccessorClient.Value;
        _clientPolicyStore = clientPolicyStore;
        _userService = userService;
        _settingsService = settingsService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExcludePath(context))
        {
            if (context.Request.Headers.TryGetValue("admin-api-key", out var extractedAdminApiKey))
            {
                string adminApiKey = _config.GetValue<string>("ApiKeys:admin-api-key");
                string adminUserId = _config.GetValue<string>("ApiKeys:admin-user-id-forAuditLogs");
                string adminRoles = _config.GetValue<string>("ApiKeys:admin-api-key-roles");
                string adminRolesDelimiter = _config.GetValue<string>("ApiKeys:admin-api-key-roles-delimiter");
                if (extractedAdminApiKey != adminApiKey)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Unauthorized key. (Using ApiKeyMiddleware)");
                    return;
                }

                string safelist = _config.GetValue<string>("ApiKeys:admin-api-key-safeList");
                bool badIp = isValidIP(context, safelist);

                if (badIp)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized IP. (Using ApiKeyMiddleware)");
                    return;
                }
                else
                {
                    var identity = new GenericIdentity(adminUserId, "admin-api-key");

                    context.User = new GenericPrincipal(identity, adminRoles.Split(adminRolesDelimiter).ToArray());

                    await defineIpRulesForApiKeys(context);
                }
            }
            else if (context.Request.Headers.TryGetValue("user-api-key", out var extractedUserApiKey))
            {
                if (!string.IsNullOrEmpty(extractedUserApiKey))
                {
                    string userApiKeyRoles = _config.GetValue<string>("ApiKeys:user-api-key-roles");
                    string userApiKeyRolesDelimiter = _config.GetValue<string>("ApiKeys:user-api-key-roles-delimiter");
                    var okResult = await _service.GetAPIKeyPairAsync(extractedUserApiKey);

                    if (okResult.Data != null)
                    {
                        APIKeyPairDto apiKeyInfo = okResult.Data as APIKeyPairDto;
                        if (apiKeyInfo != null && apiKeyInfo.ValidTill >= DateTime.Now)
                        {
                            if (!apiKeyInfo.StatusApi)
                            {
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsync("Unauthorized api key is diabled. (Using ApiKeyMiddleware)");
                                return;
                            }

                            bool badIp = isValidIP(context, apiKeyInfo.SafeListIpAddresses);

                            if (badIp)
                            {
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsync("Unauthorized IP. (Using ApiKeyMiddleware)");
                                return;
                            }
                            else
                            {
                                string useridDelimiter = _config.GetValue<string>("ApiKeys:user-api-key-userid-delimiter");
                                var identity = new GenericIdentity(apiKeyInfo.UserIds.Split(useridDelimiter).First(), "user-api-key");
                                if (apiKeyInfo.UserApiKeyModules != null && apiKeyInfo.UserApiKeyModules.Count > 0)
                                {
                                    bool ismoduleAllowed = isModuleAllowtoUserApiKey(apiKeyInfo.UserApiKeyModules, context);
                                    if (ismoduleAllowed)
                                    {
                                        context.Response.StatusCode = 401;
                                        await context.Response.WriteAsync("Unauthorized to consume this module. (Using ApiKeyMiddleware)");
                                        return;
                                    }

                                }

                                context.User = new GenericPrincipal(identity, userApiKeyRoles.Split(userApiKeyRolesDelimiter).ToArray());

                                await defineIpRulesForApiKeysSettings(context);

                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Unauthorized api key validity expired. (Using ApiKeyMiddleware)");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized api key. (Using ApiKeyMiddleware)");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Not Found client id. (Using ApiKeyMiddleware)");
                    return;
                }
            }
            else if (context.Request.Headers.TryGetValue("gen-api-key", out var extractedUserTokenApiKey))
            {
                string genApiKey = _config.GetValue<string>("ApiKeys:gen-api-key");
                if (extractedUserTokenApiKey != genApiKey)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Unauthorized key. (Using ApiKeyMiddleware)");
                    return;
                }
                else
                {
                    if (context.User?.Identity?.IsAuthenticated == false && context.GetEndpoint().DisplayName.Contains("MaintenanceController.MaintenanceMode"))
                    {
                        string genRoles = _config.GetValue<string>("ApiKeys:gen-api-key-roles");
                        string adminRolesDelimiter = _config.GetValue<string>("ApiKeys:gen-api-key-roles-delimiter");

                        string safelist = _config.GetValue<string>("ApiKeys:gen-api-key-safeList");
                        bool badIp = isValidIP(context, safelist);

                        if (badIp)
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Unauthorized IP. (Using ApiKeyMiddleware)");
                            return;
                        }
                        else
                        {
                            var identity = new GenericIdentity(Guid.Empty.ToString(), "gen-api-key");

                            context.User = new GenericPrincipal(identity, genRoles.Split(adminRolesDelimiter).ToArray());

                            await defineIpRulesForApiKeysSettings(context);
                        }
                    }
                    else if (context.User?.Identity?.IsAuthenticated == true)
                    {
                        var okResult = await _userService.GetAsync(context.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                        if (okResult.Data != null)
                        {
                            UserDetailsDto user = okResult.Data as UserDetailsDto;
                            await defineIpRulesForApiKeysSettings(context);
                        }

                    }
                }
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("api key is missing. (Using ApiKeyMiddleware)");
                return;
            }
        }

        await next(context);

    }

    private async Task defineIpRulesForApiKeys(HttpContext context)
    {
        context.Request.Headers.TryGetValue("tenant", out var tenant);
        var settings = await _settingsService.GetSettingDetailsAsync(tenant.First());
        int period = context.Items["webPortalName"].Equals("Admin") ? settings.Data.RequestsIntervalPerIPAfterLimitAdminInSeconds : settings.Data.RequestsIntervalPerIPAfterLimitClientInSeconds;
        int limit = context.Items["webPortalName"].Equals("Admin") ? settings.Data.RequestsPerIPClient : settings.Data.RequestsPerIPClient;
        if (period > 0 && limit > 0)
            await updateIpClientRateLimit(period, limit);
    }

    private async Task defineIpRulesForApiKeysSettings(HttpContext context)
    {
        context.Request.Headers.TryGetValue("tenant", out var tenant);
        var settings = await _settingsService.GetSettingDetailsAsync(tenant.First());
        int period = context.Items["webPortalName"].Equals("Client") ? settings.Data.IntervalBeforeNextAPIkeyRequestInSeconds : settings.Data.IntervalBeforeNextAPIkeyRequestInSeconds;
        int limit = context.Items["webPortalName"].Equals("Client") ? settings.Data.NumberofRequestsPerIpApiKey : settings.Data.NumberofRequestsPerIpApiKey;
        if (period > 0 && limit > 0)
            await updateIpClientRateLimit(period, limit);
    }

    private async Task updateIpClientRateLimit(int period, int limit)
    {
        try
        {
            string id = $"{_optionsClient.ClientPolicyPrefix}_gen-api-key";
            ClientRateLimitPolicy clPolicyGenApi = await _clientPolicyStore.GetAsync(id);

            if (clPolicyGenApi is null)
            {
                clPolicyGenApi = new ClientRateLimitPolicy() { ClientId = "gen-api-key" };
            }
            else if (clPolicyGenApi != null && clPolicyGenApi.Rules.Count > 0)
            {
                foreach (var item in clPolicyGenApi.Rules)
                {
                    item.Period = period == 0 ? item.Period : period + "s";
                    item.Limit = limit == 0 ? item.Limit : limit;
                }
            }

            if (clPolicyGenApi.Rules.Count == 0)
            {
                clPolicyGenApi.Rules.Add(new RateLimitRule()
                {
                    Period = period + "s",
                    Limit = limit,
                });
            }

            await _clientPolicyStore.SetAsync(id, clPolicyGenApi);

            string adminapikeyid = $"{_optionsClient.ClientPolicyPrefix}_admin-api-key";
            ClientRateLimitPolicy clPolicyAdminApi = await _clientPolicyStore.GetAsync(adminapikeyid);
            if (clPolicyAdminApi is null)
            {
                clPolicyAdminApi = new ClientRateLimitPolicy() { ClientId = "admin-api-key" };
            }
            else if (clPolicyAdminApi != null && clPolicyAdminApi.Rules.Count > 0)
            {
                foreach (var item in clPolicyAdminApi.Rules)
                {
                    item.Period = period == 0 ? item.Period : period + "s";
                    item.Limit = limit == 0 ? item.Limit : limit;
                }
            }

            if (clPolicyAdminApi.Rules.Count == 0)
            {
                clPolicyAdminApi.Rules.Add(new RateLimitRule()
                {
                    Period = period + "s",
                    Limit = limit,
                });
            }

            await _clientPolicyStore.SetAsync(adminapikeyid, clPolicyAdminApi);

            string userapikeyid = $"{_optionsClient.ClientPolicyPrefix}_user-api-key";
            ClientRateLimitPolicy clPolicyUserApiKey = await _clientPolicyStore.GetAsync(userapikeyid);
            if (clPolicyUserApiKey is null)
            {
                clPolicyUserApiKey = new ClientRateLimitPolicy() { ClientId = "user-api-key" };
            }
            else if (clPolicyUserApiKey != null && clPolicyUserApiKey.Rules.Count > 0)
            {
                foreach (var item in clPolicyUserApiKey.Rules)
                {
                    item.Period = period == 0 ? item.Period : period + "s";
                    item.Limit = limit == 0 ? item.Limit : limit;
                }
            }

            if (clPolicyUserApiKey.Rules.Count == 0)
            {
                clPolicyUserApiKey.Rules.Add(new RateLimitRule()
                {
                    Period = period + "s",
                    Limit = limit,
                });
            }

            await _clientPolicyStore.SetAsync(userapikeyid, clPolicyUserApiKey);

            var pol = await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);
            if (pol is null)
            {
                pol = new IpRateLimitPolicies();
            }
            else if (pol != null && pol.IpRules.Count > 0)
            {
                foreach (var ipRateLimitPolicy in pol.IpRules)
                {
                    foreach (var rateLimitRule in ipRateLimitPolicy.Rules)
                    {
                        rateLimitRule.Period = period == 0 ? rateLimitRule.Period : period + "s";
                        rateLimitRule.Limit = limit == 0 ? rateLimitRule.Limit : limit;
                    }
                }
            }

            if (pol.IpRules.Count == 0)
            {
                pol.IpRules.Add(new IpRateLimitPolicy()
                {
                    Ip = "*"
                });

                pol.IpRules[0].Rules.Add(new RateLimitRule()
                {
                    Period = period + "s",
                    Limit = limit,
                });
            }

            await _ipPolicyStore.SetAsync(_options.IpPolicyPrefix, pol);
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            throw;
        }
    }

    private bool isValidIP(HttpContext context, string safelist)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress;
        if (remoteIpAddress.IsIPv4MappedToIPv6)
        {
            remoteIpAddress = remoteIpAddress.MapToIPv4();
        }

        byte[] bytes = remoteIpAddress.GetAddressBytes();
        bool badIp = true;

        string[] ips = safelist.Split(';');
        byte[][] safelistBytes = new byte[ips.Length][];

        for (int i = 0; i < ips.Length; i++)
        {
            safelistBytes[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
        }

        foreach (byte[] address in safelistBytes)
        {
            if (address.SequenceEqual(bytes))
            {
                badIp = false;
                break;
            }
        }

        return badIp;
    }

    #region API Key Module Management check
    public bool isModuleAllowtoUserApiKey(ICollection<UserApiKeyModuleDto> UserApiKeyModules, HttpContext context)
    {
        bool isAllowed = true;
        if (!ExcludePath(context))
        {
            try
            {
                string module = ModuleResolver.Resolver(context);
                string moduleAction = ModuleActionResolver.Resolver(context);
                if (!string.IsNullOrEmpty(module))
                {

                    string action = moduleAction;

                    if (!string.IsNullOrEmpty(action))
                    {
                        string method = context.Request.Method;

                        if (UserApiKeyModules != null && UserApiKeyModules.Count > 0)
                        {

                            var permissionForThisController = UserApiKeyModules.FirstOrDefault(m => m.Name == module);
                            if (permissionForThisController != null && permissionForThisController.IsActive)
                            {
                                Dictionary<string, bool> permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissionForThisController.PermissionDetail);

                                bool isActive;
                                switch (method)
                                {
                                    case "GET":
                                        if (!permissions.TryGetValue(action.ToLower() == "search" ? "Search" : "View", out isActive))
                                        {
                                            isActive = false;
                                        }

                                        if (!isActive)
                                        {
                                            isAllowed = false;
                                        }

                                        break;
                                    case "POST":
                                        if (!permissions.TryGetValue(action.ToLower() == "search" ? "Search" : action.ToLower() == "register" ? "Register" : "Create", out isActive))
                                        {
                                            isActive = false;
                                        }

                                        if (!isActive)
                                        {
                                            isAllowed = false;
                                        }

                                        break;
                                    case "DELETE":
                                        if (!permissions.TryGetValue("Remove", out isActive))
                                        {
                                            isActive = false;
                                        }

                                        if (!isActive)
                                        {
                                            isAllowed = false;
                                        }

                                        break;
                                    case "PUT":
                                        if (!permissions.TryGetValue("Update", out isActive))
                                        {
                                            isActive = false;
                                        }

                                        if (!isActive)
                                        {
                                            isAllowed = false;
                                        }

                                        break;
                                    case "PATCH":
                                        if (!permissions.TryGetValue("Patch", out isActive))
                                        {
                                            isActive = false;
                                        }

                                        if (!isActive)
                                        {
                                            isAllowed = false;
                                        }

                                        break;
                                    default:
                                        isAllowed = false;
                                        break;
                                }
                            }
                            else
                            {
                                isAllowed = permissionForThisController != null && permissionForThisController.IsActive;
                            }

                        }
                        else
                        {
                            isAllowed = false;
                        }
                    }
                    else
                    {
                        isAllowed = false;
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        return isAllowed;
    }

    private bool ExcludePath(HttpContext context)
    {
        var listExclude = new List<string>()
        {
            "/files/",
            "/swagger",
            "/jobs",
            "/favicon.ico"
        };

        foreach (string item in listExclude)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains(item))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}
