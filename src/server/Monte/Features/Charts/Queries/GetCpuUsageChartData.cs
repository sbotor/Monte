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
        Guid MachineId,
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
            IReadOnlyDictionary<DateTime, double> data;

            if (!query.Core.HasValue)
            {
                var entries = await context.DbContext.MetricsEntries.AsNoTracking()
                    .OrderedFromMachineAndTime(context.MachineId, context.DateFrom, context.DateTo)
                    .ToArrayAsync(cancellationToken);

                data = context.ResultBuilder.Group(entries, x => x.ReportDateTime)
                    .WithGranularity(context.DateTimeGranularity)
                    .Apply(x => x.Cpu.AveragePercentUsed, query.AggregationType);
            }
            else
            {
                var coreEntries = await context.DbContext.CoreUsageEntries.AsNoTracking()
                    .Where(x => x.Entry.MachineId == context.MachineId)
                    .Where(x => x.Entry.ReportDateTime >= context.DateFrom
                        && x.Entry.ReportDateTime < context.DateTo)
                    .Where(x => x.Ordinal == query.Core)
                    .Select(x => new { x.Entry.ReportDateTime, x.PercentUsed })
                    .OrderBy(x => x.ReportDateTime)
                    .ToArrayAsync(cancellationToken);

                data = context.ResultBuilder.Group(coreEntries, x => x.ReportDateTime)
                    .WithGranularity(context.DateTimeGranularity)
                    .Apply(x => x.PercentUsed, query.AggregationType);
            }

            return data;
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
