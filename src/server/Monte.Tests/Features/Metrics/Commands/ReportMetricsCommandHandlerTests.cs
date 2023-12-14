using Microsoft.EntityFrameworkCore;
using Monte.Features.Machines;
using Monte.Models.Exceptions;
using Monte.Services;
using Monte.Tests.Infrastructure;
using Monte.Tests.Infrastructure.Extensions;
using NSubstitute;
using static Monte.Features.Metrics.Commands.ReportMetrics;

namespace Monte.Tests.Features.Metrics.Commands;

public class ReportMetricsCommandHandlerTests : DbTests
{
    private static readonly DateTime Now = new(2023, 12, 13, 12, 0, 0);

    private readonly Handler _sut;

    private readonly Guid _agentId = Guid.NewGuid();
    private readonly IAgentContextProvider _agentContextProvider = Substitute.For<IAgentContextProvider>();

    public ReportMetricsCommandHandlerTests()
    {
        _agentContextProvider.GetContext().Returns(new AgentContext("agent", _agentId));

        var clock = Substitute.For<IClock>();
        clock.Setup(Now);

        _sut = new(Db, _agentContextProvider, clock);

        using var db = CreateDbContext();
        db.Add(new Machine
        {
            Name = "agent",
            Id = _agentId,
            OrdinalNumber = 0,
            CreatedDateTime = Now.AddDays(-2),
            HeartbeatDateTime = Now.AddDays(-1),
            Cpu = new(),
            Memory = new()
        });
        db.SaveChanges();
    }

    [Fact]
    public async Task Handle_WhenAgentExists_AddsMetricsData()
    {
        var corePercents = new[] { 0.3, 0.33 };
        var command = new Command(
            Cpu: new() { PercentsUsed = corePercents, Load = 0.2 },
            Memory: new() { Available = 10, PercentUsed = 0.5, SwapAvailable = 2, SwapPercentUsed = 1.5 });

        await _sut.Handle(command, default);

        var machine = await Db.Machines.AsNoTracking().SingleAsync();
        Assert.Equal(Now, machine.HeartbeatDateTime);

        var entry = await Db.MetricsEntries.AsNoTracking()
            .Include(x => x.Cores)
            .SingleAsync();

        Assert.All(entry.Cores.OrderBy(x => x.Ordinal), (x, i) =>
        {
            Assert.Equal(i, x.Ordinal);
            Assert.Equal(corePercents[i], x.PercentUsed);
        });
        
        Assert.Equal(command.Cpu.Load, entry.Cpu.Load);
        Assert.Equal(command.Memory.Available, entry.Memory.Available);
        Assert.Equal(command.Memory.PercentUsed, entry.Memory.PercentUsed);
        Assert.Equal(command.Memory.SwapAvailable, entry.Memory.SwapAvailable);
        Assert.Equal(command.Memory.SwapPercentUsed, entry.Memory.SwapPercentUsed);
        
        Assert.Equal(0.315, entry.Cpu.AveragePercentUsed);
    }

    [Fact]
    public async Task Handle_WhenAgentDoesNotExist_Throws()
    {
        _agentContextProvider.GetContext().Returns(new AgentContext("agent", Guid.NewGuid()));
        var command = new Command(new(), new());

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(command, default));
    }
}
