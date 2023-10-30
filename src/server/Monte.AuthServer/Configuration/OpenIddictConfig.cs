using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;

namespace Monte.AuthServer.Configuration;

public static class OpenIddictConfig
{
    public static IEnumerable<OpenIddictScopeDescriptor> GetScopes()
    {
        yield return new()
        {
            Name = AuthConsts.Scopes.MonteMainApi
        };
        
        yield return new()
        {
            Name = AuthConsts.Scopes.MonteAgentApi
        };
    }

    public static IEnumerable<string> GetScopeNames()
        => GetScopes().Select(x => x.Name!);

    public static IEnumerable<OpenIddictApplicationDescriptor> GetApplications(
        AuthSettings settings,
        OidcAppSettings appSettings)
    {
        appSettings.Api.Validate("Invalid API OIDC config.");
        yield return new()
        {
            DisplayName = "Monte API",
            ClientId = appSettings.Api.ClientId,
            ClientSecret = appSettings.Api.ClientSecret,
            RedirectUris = { settings.RedirectUri },
            Type = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi,
            }
        };
        
        appSettings.Agent.Validate("Invalid Agent OIDC config.");
        yield return new()
        {
            DisplayName = "Monte Agent API",
            ClientId = appSettings.Agent.ClientId,
            ClientSecret = appSettings.Agent.ClientSecret,
            Type = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,

                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                OpenIddictConstants.Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteAgentApi,
            }
        };

        if (!settings.IsDevelopment)
        {
            yield break;
        }

        yield return new()
        {
            ClientId = "postman",
            ClientSecret = "postman-secret",
            RedirectUris = { new("https://localhost:7049") },
            Type = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi,
                OpenIddictConstants.Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteAgentApi,
            }
        };
    }
}
