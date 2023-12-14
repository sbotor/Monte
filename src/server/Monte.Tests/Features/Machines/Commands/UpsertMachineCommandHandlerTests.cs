using Microsoft.EntityFrameworkCore;
using Monte.Features.Machines;
using Monte.Models.Exceptions;
using Monte.Services;
using Monte.Tests.Infrastructure;
using NSubstitute;
using static Monte.Features.Machines.Commands.UpsertMachine;

namespace Monte.Tests.Features.Machines.Commands;

public class UpsertMachineCommandHandlerTests : DbTests
{
    private readonly Handler _sut;
    private readonly IAgentContextProvider _agentContextProvider = Substitute.For<IAgentContextProvider>();
    private readonly IClock _clock = Substitute.For<IClock>();

    public UpsertMachineCommandHandlerTests()
    {
        _sut = new(Db, _agentContextProvider, _clock);
        
        using var db = CreateDbContext();
        db.Add(new Machine
        {
            Name = "agent",
            OrdinalNumber = 0,
            Cpu = new(),
            Memory = new()
        });
        db.SaveChanges();
    }
    
    [Theory]
    [InlineData("new-agent", 0)]
    [InlineData("agent", 1)]
    public async Task Handle_WhenIdIsNotProvided_CreatesNewMachine(
        string origin,
        int expectedOrdinal)
    {
        _agentContextProvider.GetContext().Returns(new AgentContext(origin, null));

        var command = new Command(
            Cpu: new() { LogicalCount = 4, PhysicalCount = 64, MinFreq = 101, MaxFreq = 102 },
            Memory: new() { Total = 2, Swap = 1 });

        var existing = await Db.Machines.AsNoTracking().SingleAsync();
        
        var result = await _sut.Handle(command, default);
        var machineId = new Guid(result);
        Assert.NotEqual(existing.Id, machineId);

        var machines = await Db.Machines.AsNoTracking().ToArrayAsync();
        Assert.Equal(2, machines.Length);
        var machine = Assert.Single(machines, x => x.Id == machineId);
        
        AssertMachineData(command, machine);
        
        Assert.Equal(origin, machine.Name);
        Assert.Equal(expectedOrdinal, machine.OrdinalNumber);
    }

    [Theory]
    [InlineData("agent")]
    [InlineData("different-agent")]
    public async Task Handle_WhenIdIsProvided_UpdatesMachine(string origin)
    {
        var existing = await Db.Machines.AsNoTracking().SingleAsync();
        _agentContextProvider.GetContext().Returns(new AgentContext(origin, existing.Id));

        var command = new Command(
            Cpu: new() { LogicalCount = 4, PhysicalCount = 64, MinFreq = 101, MaxFreq = 102 },
            Memory: new() { Total = 2, Swap = 1 });
        
        var result = await _sut.Handle(command, default);
        var machineId = new Guid(result);
        Assert.Equal(existing.Id, machineId);
        
        var machines = await Db.Machines.AsNoTracking().ToArrayAsync();
        var machine = Assert.Single(machines);
        
        AssertMachineData(command, machine);
        
        Assert.Equal(existing.Name, machine.Name);
        Assert.Equal(0, machine.OrdinalNumber);
    }

    [Fact]
    public async Task Handle_WhenNonexistentIdIsProvided_Throws()
    {
        _agentContextProvider.GetContext().Returns(new AgentContext("agent", Guid.NewGuid()));
        var command = new Command(new(), new());

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(command, default));
    }

    private static void AssertMachineData(Command expected, Machine machine)
    {
        Assert.Equal(expected.Cpu.LogicalCount, machine.Cpu.LogicalCount);
        Assert.Equal(expected.Cpu.PhysicalCount, machine.Cpu.PhysicalCount);
        Assert.Equal(expected.Cpu.MinFreq, machine.Cpu.MinFreq);
        Assert.Equal(expected.Cpu.MaxFreq, machine.Cpu.MaxFreq);
        Assert.Equal(expected.Memory.Total, machine.Memory.Total);
        Assert.Equal(expected.Memory.Swap, machine.Memory.Swap);
    }
}
