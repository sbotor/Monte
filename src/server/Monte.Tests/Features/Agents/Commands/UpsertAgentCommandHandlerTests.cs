using Microsoft.EntityFrameworkCore;
using Monte.Features.Agents;
using Monte.Models.Exceptions;
using Monte.Services;
using Monte.Tests.Infrastructure;
using NSubstitute;
using static Monte.Features.Agents.Commands.UpsertAgent;

namespace Monte.Tests.Features.Agents.Commands;

public class UpsertAgentCommandHandlerTests : DbTests
{
    private readonly Handler _sut;
    private readonly IAgentContextProvider _agentContextProvider = Substitute.For<IAgentContextProvider>();
    private readonly IClock _clock = Substitute.For<IClock>();

    public UpsertAgentCommandHandlerTests()
    {
        var keyGen = Substitute.For<IMetricsKeyGenerator>();
        keyGen.GenerateKey().Returns("key");
        
        _sut = new(Db, _agentContextProvider, _clock, keyGen);
        
        using var db = CreateDbContext();
        db.Add(new Agent
        {
            Name = "agent",
            OrdinalNumber = 0,
            Cpu = new(),
            Memory = new(),
            MetricsKey = "prev key"
        });
        db.SaveChanges();
    }
    
    [Theory]
    [InlineData("new-agent", 0)]
    [InlineData("agent", 1)]
    public async Task Handle_WhenIdIsNotProvided_CreatesNewAgent(
        string origin,
        int expectedOrdinal)
    {
        _agentContextProvider.GetContext().Returns(new AgentContext(origin, null));

        var command = new Command(
            Cpu: new() { LogicalCount = 4, PhysicalCount = 64, MinFreq = 101, MaxFreq = 102 },
            Memory: new() { Total = 2, Swap = 1 });

        var existing = await Db.Agents.AsNoTracking().SingleAsync();
        
        var result = await _sut.Handle(command, default);
        var agentId = new Guid(result.AgentId);
        Assert.NotEqual(existing.Id, agentId);
        Assert.Equal("key", result.MetricsKey);

        var agents = await Db.Agents.AsNoTracking().ToArrayAsync();
        Assert.Equal(2, agents.Length);
        var agent = Assert.Single(agents, x => x.Id == agentId);
        
        AssertAgentData(command, agent);
        
        Assert.Equal(origin, agent.Name);
        Assert.Equal(expectedOrdinal, agent.OrdinalNumber);
        Assert.Equal($"{origin} #{expectedOrdinal}", agent.DisplayName);
    }

    [Theory]
    [InlineData("agent")]
    [InlineData("different-agent")]
    public async Task Handle_WhenIdIsProvided_UpdatesAgent(string origin)
    {
        var existing = await Db.Agents.AsNoTracking().SingleAsync();
        _agentContextProvider.GetContext().Returns(new AgentContext(origin, existing.Id));

        var command = new Command(
            Cpu: new() { LogicalCount = 4, PhysicalCount = 64, MinFreq = 101, MaxFreq = 102 },
            Memory: new() { Total = 2, Swap = 1 });
        
        var result = await _sut.Handle(command, default);
        var agentId = new Guid(result.AgentId);
        Assert.Equal(existing.Id, agentId);
        Assert.Equal("key", result.MetricsKey);
        
        var agents = await Db.Agents.AsNoTracking().ToArrayAsync();
        var agent = Assert.Single(agents);
        
        AssertAgentData(command, agent);
        
        Assert.Equal(existing.Name, agent.Name);
        Assert.Equal(0, agent.OrdinalNumber);
        Assert.Equal($"{existing.Name} #{existing.OrdinalNumber}", agent.DisplayName);
    }

    [Fact]
    public async Task Handle_WhenNonexistentIdIsProvided_Throws()
    {
        _agentContextProvider.GetContext().Returns(new AgentContext("agent", Guid.NewGuid()));
        var command = new Command(new(), new());

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(command, default));
    }

    private static void AssertAgentData(Command expected, Agent agent)
    {
        Assert.Equal(expected.Cpu.LogicalCount, agent.Cpu.LogicalCount);
        Assert.Equal(expected.Cpu.PhysicalCount, agent.Cpu.PhysicalCount);
        Assert.Equal(expected.Cpu.MinFreq, agent.Cpu.MinFreq);
        Assert.Equal(expected.Cpu.MaxFreq, agent.Cpu.MaxFreq);
        Assert.Equal(expected.Memory.Total, agent.Memory.Total);
        Assert.Equal(expected.Memory.Swap, agent.Memory.Swap);
    }
}
