using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Extensions;
using Monte.AuthServer.Helpers;
using OpenIddict.Abstractions;

namespace Monte.AuthServer;

public class AuthSetupWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AuthSetupWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var authSettings = _serviceProvider.GetRequiredService<IOptions<AuthSettings>>().Value;
        var appSettings = _serviceProvider.GetRequiredService<IOptions<OidcAppSettings>>().Value;

        await PopulateScopes(scope, cancellationToken);
        await PopulateApps(scope, authSettings, appSettings, cancellationToken);
        await PopulateRoles(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());
    }

    private static async Task PopulateScopes(IServiceScope scope, CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        foreach (var descriptor in OpenIddictConfig.GetCustomScopes())
        {
            await manager.UpsertScope(descriptor, cancellationToken);
        }
    }
    
    private static async Task PopulateApps(
        IServiceScope scope,
        AuthSettings authSettings,
        OidcAppSettings appSettings,
        CancellationToken cancellationToken)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var descriptor in OpenIddictConfig.GetApplications(authSettings, appSettings))
        {
            await manager.UpsertApplication(descriptor, cancellationToken);
        }
    }

    private static async Task PopulateRoles(RoleManager<IdentityRole> manager)
    {
        var role = await manager.FindByNameAsync(AuthConsts.Roles.MonteAdmin);
        
        if (role is not null)
        {
            return;
        }
        
        await manager.CreateAsync(new() { Name = AuthConsts.Roles.MonteAdmin });
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
