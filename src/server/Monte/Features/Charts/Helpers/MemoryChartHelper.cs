using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Monte.Features.Charts.Models;
using Monte.Features.Metrics;
using Monte.Features.Metrics.Extensions;

namespace Monte.Features.Charts.Helpers;

public interface IMemoryChartRequest : IChartRequest
{
    bool Swap { get; }
    ChartAggregationType AggregationType { get; }
}

public interface IMemoryChartHelper<in TRequest> where TRequest : IMemoryChartRequest
{
    Task<ChartData<double>> GetMemoryUsageData(TRequest request, CancellationToken cancellationToken);
    Task<ChartData<double>> GetAvailableMemoryData(TRequest request, CancellationToken cancellationToken);
}

public class MemoryChartHelper<TRequest> : IMemoryChartHelper<TRequest> where TRequest : IMemoryChartRequest
{
    private readonly IChartHelper<TRequest> _helper;

    public MemoryChartHelper(IChartHelper<TRequest> helper)
    {
        _helper = helper;
    }

    public Task<ChartData<double>> GetMemoryUsageData(TRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<MetricsEntry, RawChartValue>> rawSelector = request.Swap
            ? x => new RawChartValue(x.ReportDateTime, x.Memory.SwapPercentUsed)
            : x => new RawChartValue(x.ReportDateTime, x.Memory.PercentUsed);

        return _helper.FetchData(request,
            x => FetchValues(x, request, rawSelector, cancellationToken));
    }

    public Task<ChartData<double>> GetAvailableMemoryData(TRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<MetricsEntry, RawChartValue>> rawSelector = request.Swap
            ? x => new RawChartValue(x.ReportDateTime, x.Memory.SwapAvailable)
            : x => new RawChartValue(x.ReportDateTime, x.Memory.Available);

        return _helper.FetchData(request,
            x => FetchValues(x, request, rawSelector, cancellationToken));
    }

    private static async Task<IReadOnlyDictionary<DateTime, double>> FetchValues(
        ChartContext context,
        TRequest request,
        Expression<Func<MetricsEntry, RawChartValue>> rawResultSelector,
        CancellationToken cancellationToken)
    {
        var entries = await context.DbContext.MetricsEntries.AsNoTracking()
            .OrderedFromAgentAndTime(context.AgentId, context.DateFrom, context.DateTo)
            .Select(rawResultSelector)
            .ToArrayAsync(cancellationToken);

        return context.ResultAggregator.AggregateValues(entries,
            context.DateTimeGranularity, request.AggregationType);
    }
}
