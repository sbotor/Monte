using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monte.Behaviors;
using Monte.Features.Charts.Helpers;
using Monte.Services;

[assembly: InternalsVisibleTo("Monte.Tests")]

namespace Monte;

public static class Setup
{
    private static readonly Assembly ThisAssembly = typeof(Setup).Assembly;
    
    public static IServiceCollection AddMonte(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(ThisAssembly);

            x.AddOpenBehavior(typeof(AgentValidationBehavior<,>));
            x.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(ThisAssembly);

        services.AddDbContext<MonteDbContext>(
            x => x.UseNpgsql(config.GetConnectionString("Default")));

        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<IChartResultAggregator, ChartResultAggregator>();
        services.AddSingleton<IMetricsKeyGenerator, MetricsKeyGenerator>();

        services.AddScoped(typeof(IChartHelper<>), typeof(ChartHelper<>));
        services.AddScoped(typeof(IMemoryChartHelper<>), typeof(MemoryChartHelper<>));
        
        return services;
    }
}
