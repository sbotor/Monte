using OpenIddict.Abstractions;

namespace Monte.AuthServer.Configuration;

public static class OpenIddictConfig
{
    public static IEnumerable<OpenIddictScopeDescriptor> GetScopes()
    {
        yield return new()
        {
            Name = "monte_api"
        };
    }

    public static IEnumerable<OpenIddictApplicationDescriptor> GetApplications(AuthSettings settings)
    {
        yield return new()
        {
            ClientId = "monte_api",
            ClientSecret = "secret",
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

                OpenIddictConstants.Permissions.Prefixes.Scope + "monte_api"
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

                OpenIddictConstants.Permissions.Prefixes.Scope + "monte_api"
            }
        };
    }
}