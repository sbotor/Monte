using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Monte.WebApi.Configuration;
using OpenIddict.Validation.AspNetCore;

namespace Monte.WebApi.Auth;

public static class AuthSetup
{
    public const string RequireMainApiScope = "MonteMainApi";
    public const string RequireAgentApiScope = "MonteClientApi";
    
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

        services.AddAuthorization(x =>
        {
            x.AddPolicy(RequireMainApiScope, y => y.RequireScope("monte_main_api"));
            x.AddPolicy(RequireAgentApiScope, y => y.RequireScope("monte_agent_api"));

            x.DefaultPolicy = x.GetPolicy(RequireMainApiScope)!;
        });
    }

    private static void RequireScope(this AuthorizationPolicyBuilder builder, string scope)
        => builder.RequireAuthenticatedUser()
            .RequireAssertion(ctx =>
            {
                var scopeClaim = ctx.User.FindFirst("scope");
                return scopeClaim is not null
                       && scopeClaim.Value.Split(' ').Any(x => x == scope);
            });
}
