using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Data;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Monte.AuthServer;

internal static class Setup
{
    public static void RegisterServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddDbContext<AuthDbContext>(x =>
        {
            x.UseNpgsql(config.GetConnectionString("Default"));
            x.UseOpenIddict();
        });
        
        services.AddIdentity();

        services.ConfigureOpenIddict(config);
        
        services.ConfigureApplicationCookie(x =>
        {
            x.LoginPath = "/Login";
        });
        
        services.Configure<AuthSettings>(config.GetSection(nameof(AuthSettings)));
        services.Configure<OidcAppSettings>(config.GetSection(nameof(OidcAppSettings)));
        
        services.AddHostedService<AuthSetupWorker>();

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUserService, UserService>();
    }

    private static void AddIdentity(this IServiceCollection services)
        => services.AddIdentity<AppUser, AppRole>(x =>
            {
                x.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                x.ClaimsIdentity.UserNameClaimType = Claims.Name;
                x.ClaimsIdentity.RoleClaimType = Claims.Role;
            })
            .AddEntityFrameworkStores<AuthDbContext>();

    private static void ConfigureOpenIddict(this IServiceCollection services,
        IConfiguration config)
    {
        var settings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException("Token settings not found.");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        
        services.AddOpenIddict()
            .AddCore(x =>
            {
                x.UseEntityFrameworkCore()
                    .UseDbContext<AuthDbContext>();
            })
            .AddServer(x =>
            {
                x.AllowAuthorizationCodeFlow()
                    .AllowClientCredentialsFlow();

                x.AddSigningKey(key);
                
                x.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetUserinfoEndpointUris("connect/user-info", "connect/userinfo");

                var lifetimes = settings.Lifetimes;
                x.SetAuthorizationCodeLifetime(lifetimes.AuthCode)
                    .SetAccessTokenLifetime(lifetimes.AccessToken);

                x.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate()
                    .DisableAccessTokenEncryption();

                var scopes = OpenIddictConfig.GetCustomScopeNames()
                    .Concat(new[] { Scopes.Roles, Scopes.Profile })
                    .ToArray();
                x.RegisterScopes(scopes);

                var aspNetCoreBuilder = x.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough();

                if (bool.TryParse(config["AllowHttp"], out var allowHttp) && allowHttp)
                {
                    aspNetCoreBuilder.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(x =>
            {
                x.SetIssuer(settings.Issuer)
                    .SetConfiguration(new() { Issuer = settings.Issuer, SigningKeys = { key } })
                    .UseAspNetCore();
            });
        
        services.AddAuthentication(AuthSchemes.TokenValidation);
        services.AddAuthorization();
    }
}
