using Monte.Features.Charts;
using Monte.Features.Charts.Queries;

namespace Monte.WebApi.Features.Charts.Requests;

public class GetCpuUsageChartDataRequest
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public ChartAggregationType AggregationType { get; set; }
    public int? Core { get; set; }

    public GetCpuUsageChartData.Query ToQuery(Guid agentId)
        => new(agentId, DateFrom, DateTo, AggregationType, Core);
}
