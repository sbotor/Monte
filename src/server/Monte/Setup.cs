using Microsoft.Extensions.DependencyInjection;

namespace Monte;

public static class Setup
{
    public static IServiceCollection AddMonte<TUserContext>(this IServiceCollection services)
        where TUserContext : class, IUserContext
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(Setup).Assembly);
        });

        services.AddScoped<IUserContext, TUserContext>();
        
        return services;
    }
}