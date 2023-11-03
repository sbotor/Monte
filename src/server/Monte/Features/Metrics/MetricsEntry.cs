using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monte.Features.Machines;

namespace Monte.Features.Metrics;

public class MetricsEntry
{
    public uint Id { get; set; }
    public DateTime ReportDateTime { get; set; }

    public ICollection<CoreUsageEntry> Cores { get; set; } = new List<CoreUsageEntry>();
    
    public Guid MachineId { get; set; }
}

public class CoreUsageEntry
{
    public int Ordinal { get; set; }
    public double PercentUsed { get; set; }
}

internal class MetricsEntryConfiguration : IEntityTypeConfiguration<MetricsEntry>
{
    public void Configure(EntityTypeBuilder<MetricsEntry> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(x => x.MachineId);

        builder.HasMany(x => x.Cores)
            .WithOne();
    }
}
