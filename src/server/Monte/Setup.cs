using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monte.Behaviors;

namespace Monte;

public static class Setup
{
    private static readonly Assembly ThisAssembly = typeof(Setup).Assembly;
    
    public static IServiceCollection AddMonte(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(ThisAssembly);

            x.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(ThisAssembly);

        services.AddDbContext<MonteDbContext>(
            x => x.UseNpgsql(config.GetConnectionString("Default")));

        services.AddSingleton<IClock, Clock>();
        
        return services;
    }
}
