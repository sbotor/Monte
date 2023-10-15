using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Helpers;

namespace Monte.AuthServer;

internal static class Setup
{
    public static void RegisterServices(this IServiceCollection services, IConfigurationRoot config)
    {
        var connectionString = config.GetConnectionString("Default");
        services.AddDbContext<AuthDbContext>(x =>
        {
            x.UseSqlite(connectionString);
            x.UseOpenIddict();
        });
        
        services.AddIdentity();

        var tokenSettings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException("Token settings not found.");
        
        services.ConfigureOpenIddict(tokenSettings);
        
        
        services.ConfigureApplicationCookie(x =>
        {
            x.LoginPath = "/Login";
        });
        
        services.Configure<AuthSettings>(config.GetSection(nameof(AuthSettings)));
        
        services.AddHostedService<AuthSetupWorker>();
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AuthDbContext>();
    }

    private static void ConfigureOpenIddict(this IServiceCollection services,
        TokenSettings settings)
    {
        services.AddOpenIddict()
            .AddCore(x =>
            {
                x.UseEntityFrameworkCore()
                    .UseDbContext<AuthDbContext>();
            })
            .AddServer(x =>
            {
                x.AllowAuthorizationCodeFlow();
                
                x.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetUserinfoEndpointUris("connect/user-info");

                var lifetimes = settings.Lifetimes;
                x.SetAuthorizationCodeLifetime(lifetimes.AuthCode)
                    .SetAccessTokenLifetime(lifetimes.AccessToken);

                x.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate()
                    .DisableAccessTokenEncryption();

                x.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough();
            });
    }
}