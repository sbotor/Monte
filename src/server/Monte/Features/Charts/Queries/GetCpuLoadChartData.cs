using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Features.Charts.Helpers;
using Monte.Features.Charts.Models;
using Monte.Features.Metrics.Extensions;

namespace Monte.Features.Charts.Queries;

public static class GetCpuLoadChartData
{
    public record Query(
        Guid AgentId,
        DateTime DateFrom,
        DateTime DateTo,
        ChartAggregationType AggregationType)
        : IRequest<ChartData<double>>, IChartRequest;

    internal class Handler : IRequestHandler<Query, ChartData<double>>
    {
        private readonly IChartHelper<Query> _helper;

        public Handler(IChartHelper<Query> helper)
        {
            _helper = helper;
        }
        
        public Task<ChartData<double>> Handle(Query request, CancellationToken cancellationToken)
            => _helper.FetchData(request, x => FetchValues(x, request, cancellationToken));

        private static async Task<IReadOnlyDictionary<DateTime, double>> FetchValues(
            ChartContext context,
            Query query,
            CancellationToken cancellationToken)
        {
            var entries = await context.DbContext.MetricsEntries.AsNoTracking()
                .OrderedFromAgentAndTime(context.AgentId, context.DateFrom, context.DateTo)
                .Select(x => new RawChartValue(x.ReportDateTime, x.Cpu.Load))
                .ToArrayAsync(cancellationToken);

            return context.ResultAggregator.AggregateValues(entries,
                context.DateTimeGranularity, query.AggregationType);
        }
    }
}
