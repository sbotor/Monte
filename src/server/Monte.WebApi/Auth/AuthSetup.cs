using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Monte.WebApi.Configuration;

namespace Monte.WebApi.Auth;

public static class AuthSetup
{
    public static string MonteMainApiPolicy = "MonteMainApi";
    public static string MonteClientApiPolicy = "MonteClientApi";
    
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
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization(x =>
        {
        });
    }

    private static void RequireScope(this AuthorizationPolicyBuilder builder)
    {
    }
}