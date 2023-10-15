using OpenIddict.Abstractions;

namespace Monte.AuthServer.Extensions;

public static class OpenIddictExtensions
{
    public static async Task UpsertScope(this IOpenIddictScopeManager manager,
        OpenIddictScopeDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        var instance = await manager.FindByNameAsync(descriptor.Name!, cancellationToken);

        if (instance is null)
        {
            await manager.CreateAsync(descriptor, cancellationToken);
        }
        else
        {
            await manager.UpdateAsync(instance, descriptor, cancellationToken);
        }
    }
    
    public static async Task UpsertApplication(this IOpenIddictApplicationManager manager,
        OpenIddictApplicationDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        var instance = await manager.FindByClientIdAsync(descriptor.ClientId!, cancellationToken);

        if (instance is null)
        {
            await manager.CreateAsync(descriptor, cancellationToken);
        }
        else
        {
            await manager.UpdateAsync(instance, descriptor, cancellationToken);
        }
    }
}