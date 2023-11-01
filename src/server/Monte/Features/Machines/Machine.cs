using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Monte.Helpers;

namespace Monte.Features.Machines;

public class Machine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public int OrdinalNumber { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime HeartbeatDateTime { get; set; }
    public CpuInfo Cpu { get; set; } = null!;
    public MemoryInfo Memory { get; set; } = null!;

    public string DisplayName => $"{Name} #{OrdinalNumber}";
    
    public class CpuInfo
    {
        public int LogicalCount { get; set; }
        public int PhysicalCount { get; set; }
        public double MinFreq { get; set; }
        public double MaxFreq { get; set; }

        public void Update(CpuInfo other)
        {
            LogicalCount = other.LogicalCount;
            PhysicalCount = other.PhysicalCount;
            MinFreq = other.MinFreq;
            MaxFreq = other.MaxFreq;
        }
    }

    public class MemoryInfo
    {
        public ulong Total { get; set; }
        public ulong Swap { get; set; }

        public void Update(MemoryInfo other)
        {
            Total = other.Total;
            Swap = other.Swap;
        }
    }
}

internal class MachineEntityConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Length.MachineName.Max);

        builder.OwnsOne(x => x.Cpu);
        builder.OwnsOne(x => x.Memory);
    }
}
