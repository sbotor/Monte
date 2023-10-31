using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Monte;

public static class Setup
{
    public static IServiceCollection AddMonte(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(Setup).Assembly);
        });

        services.AddDbContext<MonteDbContext>(
            x => x.UseNpgsql(config.GetConnectionString("Default")));

        services.AddSingleton<IClock, Clock>();
        
        return services;
    }
}
