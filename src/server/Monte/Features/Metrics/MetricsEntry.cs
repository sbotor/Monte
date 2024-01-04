using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monte.Features.Agents;

namespace Monte.Features.Metrics;

public class MetricsEntry
{
    public uint Id { get; set; }
    public DateTime ReportDateTime { get; set; }
    public CpuUsageEntry Cpu { get; set; } = null!;
    public MemoryUsageEntry Memory { get; set; } = null!;

    public ICollection<CoreUsageEntry> Cores { get; set; } = new List<CoreUsageEntry>();
    
    public Guid AgentId { get; set; }
}

public class CoreUsageEntry
{
    public int Ordinal { get; set; }
    public double PercentUsed { get; set; }
    
    public uint EntryId { get; set; }
    public MetricsEntry Entry { get; set; } = null!;
}

public class CpuUsageEntry
{
    public double Load { get; set; }
    public double AveragePercentUsed { get; set; }
}

public class MemoryUsageEntry
{
    public double Available { get; set; }
    public double PercentUsed { get; set; }
    public double SwapAvailable { get; set; }
    public double SwapPercentUsed { get; set; }
}

internal class MetricsEntryConfiguration : IEntityTypeConfiguration<MetricsEntry>
{
    public void Configure(EntityTypeBuilder<MetricsEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Cpu);
        builder.OwnsOne(x => x.Memory);
        
        builder.HasOne<Agent>()
            .WithMany()
            .HasForeignKey(x => x.AgentId);
    }
}

internal class CoreUsageEntryConfiguration : IEntityTypeConfiguration<CoreUsageEntry>
{
    public void Configure(EntityTypeBuilder<CoreUsageEntry> builder)
    {
        builder.HasKey(x => new { x.EntryId, x.Ordinal });
        
        builder.HasOne(x => x.Entry)
            .WithMany(x => x.Cores)
            .HasForeignKey(x => x.EntryId);
    }
}
