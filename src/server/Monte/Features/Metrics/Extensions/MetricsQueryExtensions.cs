namespace Monte.Features.Metrics.Extensions;

public static class MetricsQueryExtensions
{
    public static IQueryable<MetricsEntry> OrderedFromMachineAndTime(
        this IQueryable<MetricsEntry> query,
        Guid machineId,
        DateTime from,
        DateTime to)
        => query.Where(x => x.MachineId == machineId)
            .Where(x => x.ReportDateTime >= from
                && x.ReportDateTime < to)
            .OrderBy(x => x.ReportDateTime);
}
