using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Monte.Features.Machines;

namespace Monte;

public class MonteDbContext : DbContext
{
    public DbSet<Machine> Machines => Set<Machine>();
    
    public MonteDbContext(DbContextOptions<MonteDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MachineEntityConfiguration());
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