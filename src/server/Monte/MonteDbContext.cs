using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Monte.Features.Agents;
using Monte.Features.Metrics;

namespace Monte;

public class MonteDbContext : DbContext
{
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<MetricsEntry> MetricsEntries => Set<MetricsEntry>();
    public DbSet<CoreUsageEntry> CoreUsageEntries => Set<CoreUsageEntry>();
    
    public MonteDbContext(DbContextOptions<MonteDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AgentEntityConfiguration());

        modelBuilder.ApplyConfiguration(new MetricsEntryConfiguration());
        modelBuilder.ApplyConfiguration(new CoreUsageEntryConfiguration());
    }
}

internal class MonteDbContextDesignFactory : IDesignTimeDbContextFactory<MonteDbContext>
{
    public MonteDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MonteDbContext>()
            .UseNpgsql("Host=127.0.0.1:5432;Database=Monte;Username=postgres;Password=postgres;")
            .Options;

        return new(options);
    }
}
