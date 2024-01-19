namespace Monte.Features.Agents.Models;

public record AgentDetails(
    Guid Id,
    DateTime LastHeartbeat,
    string DisplayName,
    Agent.CpuInfo Cpu,
    Agent.MemoryInfo Memory);
