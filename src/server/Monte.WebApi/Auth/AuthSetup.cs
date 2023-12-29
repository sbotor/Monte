using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Monte.WebApi.Configuration;
using OpenIddict.Validation.AspNetCore;

namespace Monte.WebApi.Auth;

public static class AuthConsts
{
    public static class Roles
    {
        public const string MonteAgent = "monte_agent";
        public const string MonteAdmin = "monte_admin";
        public const string MonteUser = "monte_user";
    }

    public static class Groups
    {
        public const string AllUsers = $"{Roles.MonteAdmin},{Roles.MonteUser}";
    }
}

public static class AuthSetup
{
    public static void ConfigureAuth(this IServiceCollection services, IConfigurationRoot config)
    {
        var settings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException("No token settings found");

        services.AddOpenIddict(x =>
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            x.AddValidation().SetIssuer(settings.Issuer)
                .SetConfiguration(new() { Issuer = settings.Issuer, SigningKeys = { key } })
                .UseAspNetCore();
        });
        
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();
    }
}
