using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Monte.AuthServer.Configuration;

public static class OpenIddictConfig
{
    public static IEnumerable<OpenIddictScopeDescriptor> GetCustomScopes()
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

    public static IEnumerable<string> GetCustomScopeNames()
        => GetCustomScopes().Select(x => x.Name!);

    public static IEnumerable<OpenIddictApplicationDescriptor> GetApplications(
        AuthSettings settings,
        OidcAppSettings appSettings)
    {
        appSettings.Client.Validate("Invalid Client OIDC config.", true);
        yield return new()
        {
            DisplayName = "Monte Client",
            ClientId = appSettings.Client.ClientId,
            RedirectUris = { settings.RedirectUri },
            PostLogoutRedirectUris = { settings.RedirectUri },
            Type = ClientTypes.Public,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Logout,

                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.ResponseTypes.Code,

                Permissions.Scopes.Roles,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        };
        
        appSettings.Agent.Validate("Invalid Agent OIDC config.");
        yield return new()
        {
            DisplayName = "Monte Agent",
            ClientId = appSettings.Agent.ClientId,
            ClientSecret = appSettings.Agent.ClientSecret,
            Type = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,

                Permissions.GrantTypes.ClientCredentials,

                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteAgentApi,
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
            Type = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Logout,
                
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.ClientCredentials,
                
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Roles,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi,
                Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteAgentApi
            }
        };
        
        yield return new()
        {
            ClientId = "swagger-ui",
            RedirectUris = { new("https://localhost:7048/swagger/oauth2-redirect.html"), new("https://localhost:7049/swagger/oauth2-redirect.html") },
            Type = ClientTypes.Public,
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.Endpoints.Logout,
                
                Permissions.GrantTypes.AuthorizationCode,
                
                Permissions.ResponseTypes.Code,
                
                Permissions.Scopes.Roles,
                Permissions.Scopes.Profile,
                Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi,
            }
        };
    }
}
