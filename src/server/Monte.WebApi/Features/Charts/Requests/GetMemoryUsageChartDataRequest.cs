using Monte.Features.Charts;
using Monte.Features.Charts.Queries;

namespace Monte.WebApi.Features.Charts.Requests;

public class GetMemoryUsageChartDataRequest
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public ChartAggregationType AggregationType { get; set; }
    public bool Swap { get; set; }

    public GetMemoryUsageChartData.Query ToQuery(Guid machineId)
        => new(machineId, DateFrom, DateTo, AggregationType, Swap);
}
