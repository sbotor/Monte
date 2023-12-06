using Monte.Features.Charts.Queries;

namespace Monte.WebApi.Features.Charts.Requests;

public class GetAvgCpuUsageChartDataRequest
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public GetAvgCpuUsageChartData.Query ToQuery(Guid machineId)
        => new(machineId, DateFrom, DateTo);
}
