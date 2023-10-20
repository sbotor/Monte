using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Monte.WebApi.Configuration;

namespace Monte.WebApi.Auth;

public static class AuthSetup
{
    public const string RequireMainApiScope = "MonteMainApi";
    public const string RequireAgentApiScope = "MonteClientApi";
    
    public static void ConfigureAuth(this IServiceCollection services, IConfigurationRoot config)
    {
        var settings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException("No token settings found");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

                x.TokenValidationParameters = new()
                {
                    ValidIssuer = settings.Issuer,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                };
            });

        services.AddAuthorization(x =>
        {
            x.AddPolicy(RequireMainApiScope, y => y.RequireScope("monte_main_api"));
            x.AddPolicy(RequireAgentApiScope, y => y.RequireScope("monte_agent_api"));

            x.DefaultPolicy = x.GetPolicy(RequireMainApiScope)!;
        });
    }

    private static void RequireScope(this AuthorizationPolicyBuilder builder, string scope)
    {
        builder.RequireAuthenticatedUser()
            .RequireAssertion(ctx =>
            {
                var scopeClaim = ctx.User.FindFirst("scope");
                return scopeClaim is not null
                    && scopeClaim.Value.Split(' ').Any(x => x == scope);
            });
    }
}