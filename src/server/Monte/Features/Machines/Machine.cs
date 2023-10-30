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

    public string DisplayName => $"{Name} #{OrdinalNumber}";
}

internal class MachineEntityConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Length.MachineName.Max);
    }
}
