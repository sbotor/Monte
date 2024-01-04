using Monte.Contracts;
using Monte.Extensions;
using Monte.Features.Charts.Models;

namespace Monte.Features.Charts.Helpers;

public record ChartContext(
    MonteDbContext DbContext,
    Guid AgentId,
    DateTime DateFrom,
    DateTime DateTo,
    DateTimeGranularity DateTimeGranularity,
    IChartResultBuilder ResultBuilder);

public interface IChartRequest : IDateRange
{
    Guid AgentId { get; }
}

public interface IChartHelper<in TRequest> where TRequest : IChartRequest
{
    Task<ChartData<double>> FetchData(
        TRequest request,
        Func<ChartContext, Task<IReadOnlyDictionary<DateTime, double>>> resultFunc);
}

public class ChartHelper<TRequest> : IChartHelper<TRequest> where TRequest : IChartRequest
{
    private readonly MonteDbContext _dbContext;
    private readonly IChartResultBuilder _resultBuilder;

    public ChartHelper(MonteDbContext dbContext, IChartResultBuilder resultBuilder)
    {
        _dbContext = dbContext;
        _resultBuilder = resultBuilder;
    }

    public Task<ChartData<double>> FetchData(
        TRequest request,
        Func<ChartContext, Task<IReadOnlyDictionary<DateTime, double>>> resultFunc)
        => FetchData(request, resultFunc, 0d);

    private async Task<ChartData<TValues>> FetchData<TValues>(
        TRequest request,
        Func<ChartContext, Task<IReadOnlyDictionary<DateTime, TValues>>> resultFunc,
        TValues defaultValue)
    {
        var from = request.DateFrom.ToUniversalTime();
        var to = request.DateTo.ToUniversalTime().RoundToNextDay();
        var timeDiffKind = to.GetGranularity(from);

        var context = new ChartContext(
            _dbContext,
            request.AgentId,
            from,
            to,
            timeDiffKind,
            _resultBuilder);

        var data = await resultFunc(context);

        var chartData = new ChartData<TValues>(from.EnumerateUntil(to, timeDiffKind));
        chartData.Collect(data, defaultValue);

        return chartData;
    }
}
