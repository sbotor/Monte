using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monte.Cqrs.Behaviors;

namespace Monte;

public static class Setup
{
    public static IServiceCollection AddMonte(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(Setup).Assembly);

            x.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
        });

        services.AddDbContext<MonteDbContext>(
            x => x.UseNpgsql(config.GetConnectionString("Default")));

        services.AddValidatorsFromAssembly(typeof(Setup).Assembly, includeInternalTypes: true);

        services.AddSingleton<IClock, Clock>();
        
        return services;
    }
}
