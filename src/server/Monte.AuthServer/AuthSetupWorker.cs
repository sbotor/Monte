using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monte.AuthServer.Configuration;
using Monte.AuthServer.Data;
using Monte.AuthServer.Extensions;
using Monte.AuthServer.Helpers;
using Monte.AuthServer.Models;
using Monte.AuthServer.Services;
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
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AuthDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var authSettings = _serviceProvider.GetRequiredService<IOptions<AuthSettings>>().Value;
        var appSettings = _serviceProvider.GetRequiredService<IOptions<OidcAppSettings>>().Value;

        await PopulateScopes(scope, cancellationToken);
        await PopulateApps(scope, authSettings, appSettings, cancellationToken);
        await PopulateRoles(services.GetRequiredService<RoleManager<AppRole>>());
        await EnsureAdmin(services.GetRequiredService<IUserService>());
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

    private static async Task PopulateRoles(RoleManager<AppRole> manager)
    {
        var admin = await manager.FindByNameAsync(AuthConsts.Roles.MonteAdmin);
        var user = await manager.FindByNameAsync(AuthConsts.Roles.MonteUser);

        if (user == null)
        {
            await manager.CreateAsync(new() { Name = AuthConsts.Roles.MonteUser });
        }

        if (admin == null)
        {
            await manager.CreateAsync(new() { Name = AuthConsts.Roles.MonteAdmin });
        }
    }

    private static async Task EnsureAdmin(IUserService userService)
    {
        var result = await userService.CreateUser(new() { Username = "admin", Password = "admin@DM1N" },
            AuthConsts.Roles.MonteAdmin);

        if (result.ErrType != ErrorType.None)
        {
            throw new InvalidOperationException($"Could not create admin. Error: '{result.ErrorMessage}'");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
