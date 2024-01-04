namespace Monte.Features.Metrics.Extensions;

public static class MetricsQueryExtensions
{
    public static IQueryable<MetricsEntry> OrderedFromAgentAndTime(
        this IQueryable<MetricsEntry> query,
        Guid agentId,
        DateTime from,
        DateTime to)
        => query.Where(x => x.AgentId == agentId)
            .Where(x => x.ReportDateTime >= from
                && x.ReportDateTime < to)
            .OrderBy(x => x.ReportDateTime);
}
