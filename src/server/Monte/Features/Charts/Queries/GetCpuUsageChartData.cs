using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Contracts;
using Monte.Features.Charts.Helpers;
using Monte.Features.Charts.Models;
using Monte.Features.Metrics.Extensions;

namespace Monte.Features.Charts.Queries;

public static class GetCpuUsageChartData
{
    public record Query(
        Guid AgentId,
        DateTime DateFrom,
        DateTime DateTo,
        ChartAggregationType AggregationType,
        int? Core)
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
            RawChartValue[] data;

            if (!query.Core.HasValue)
            {
                data = await context.DbContext.MetricsEntries.AsNoTracking()
                    .OrderedFromAgentAndTime(context.AgentId, context.DateFrom, context.DateTo)
                    .Select(x => new RawChartValue(x.ReportDateTime, x.Cpu.AveragePercentUsed))
                    .ToArrayAsync(cancellationToken);
            }
            else
            {
                data = await context.DbContext.CoreUsageEntries.AsNoTracking()
                    .Where(x => x.Entry.AgentId == context.AgentId)
                    .Where(x => x.Entry.ReportDateTime >= context.DateFrom
                                && x.Entry.ReportDateTime < context.DateTo)
                    .Where(x => x.Ordinal == query.Core)
                    .Select(x => new RawChartValue(x.Entry.ReportDateTime, x.PercentUsed))
                    .OrderBy(x => x.ReportDateTime)
                    .ToArrayAsync(cancellationToken);
            }

            return context.ResultAggregator.AggregateValues(data,
                context.DateTimeGranularity, query.AggregationType);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            Include(new DateRangeValidator());
        }
    }
}
