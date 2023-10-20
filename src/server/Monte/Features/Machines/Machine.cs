using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monte.Features.Machines;

public class Machine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = null!;
}

internal class MachineEntityConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.HasKey(x => x.Id);
    }
}