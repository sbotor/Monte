using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Contracts;
using Monte.Features.Charts.Helpers;
using Monte.Features.Charts.Models;
using Monte.Features.Metrics;
using Monte.Features.Metrics.Extensions;

namespace Monte.Features.Charts.Queries;

public static class GetMemoryUsageChartData
{
    public record Query(
        Guid MachineId,
        DateTime DateFrom,
        DateTime DateTo,
        ChartAggregationType AggregationType,
        bool Swap)
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
                .OrderedFromMachineAndTime(context.MachineId, context.DateFrom, context.DateTo)
                .ToArrayAsync(cancellationToken);

            Func<MetricsEntry, double> valueSelector = query.Swap
                ? x => x.Memory.PercentUsed
                : x => x.Memory.SwapPercentUsed;

            return context.ResultBuilder.Group(entries, x => x.ReportDateTime)
                .WithGranularity(context.DateTimeGranularity)
                .Apply(valueSelector, query.AggregationType);
        }
    }

    public class Validator : AbstractValidator<GetCpuUsageChartData.Query>
    {
        public Validator()
        {
            Include(new DateRangeValidator());
        }
    }
}
