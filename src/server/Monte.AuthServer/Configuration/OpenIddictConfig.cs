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

    public static IEnumerable<OpenIddictApplicationDescriptor> GetApplications(AuthSettings settings)
    {
        yield return new()
        {
            DisplayName = "Monte API",
            ClientId = AuthConsts.ClientIds.MonteApi,
            ClientSecret = AuthConsts.ClientSecrets.MonteApi,
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
        
        yield return new()
        {
            DisplayName = "Monte Agent API",
            ClientId = AuthConsts.ClientIds.MonteAgent,
            ClientSecret = AuthConsts.ClientSecrets.MonteAgent,
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
                
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                OpenIddictConstants.Permissions.Prefixes.Scope + AuthConsts.Scopes.MonteMainApi,
            }
        };
    }
}