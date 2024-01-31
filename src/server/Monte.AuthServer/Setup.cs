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
        var tokenSettings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException("Token settings not found.");
        
        var authSettings = config.GetSection(nameof(AuthSettings)).Get<AuthSettings>()
            ?? throw new InvalidOperationException("Auth settings not found.");

        var allowHttp = bool.TryParse(config["AllowHttp"], out var allowHttpValue) && allowHttpValue;
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SigningKey));
        
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

                var lifetimes = tokenSettings.Lifetimes;
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

                if (allowHttp)
                {
                    aspNetCoreBuilder.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(x =>
            {
                x.SetIssuer(tokenSettings.Issuer)
                    .SetConfiguration(new() { Issuer = tokenSettings.Issuer, SigningKeys = { key } })
                    .UseAspNetCore();
            })
            .AddClient(x =>
            {
                x.AllowAuthorizationCodeFlow();

                x.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                var aspNetCoreBuilder = x.UseAspNetCore()
                    .EnableRedirectionEndpointPassthrough();

                if (allowHttp)
                {
                    aspNetCoreBuilder.DisableTransportSecurityRequirement();
                }

                x.UseWebProviders()
                    .AddGoogle(y =>
                    {
                        var googleSettings = authSettings.Google;
                        y.SetClientId(googleSettings.ClientId)
                            .SetClientSecret(googleSettings.ClientSecret)
                            .SetRedirectUri(googleSettings.RedirectUri)
                            .AddScopes("email");
                    });
            });
        
        services.AddAuthentication(AuthSchemes.TokenValidation);
        services.AddAuthorization();
    }
}
