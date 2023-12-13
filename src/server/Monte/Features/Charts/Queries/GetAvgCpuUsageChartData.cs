using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monte.Contracts;
using Monte.Extensions;
using Monte.Features.Charts.Models;

namespace Monte.Features.Charts.Queries;

public static class GetAvgCpuUsageChartData
{
    public record Query(Guid MachineId, DateTime DateFrom, DateTime DateTo, int? Core)
        : IRequest<ChartData<double>>, IDateRange;

    internal class Handler : IRequestHandler<Query, ChartData<double>>
    {
        private readonly MonteDbContext _dbContext;

        public Handler(MonteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChartData<double>> Handle(Query request, CancellationToken cancellationToken)
        {
            var from = request.DateFrom.ToUniversalTime();
            var to = request.DateTo.ToUniversalTime().RoundToNextDay();
            
            var timeDiffKind = to.GetDiffKind(from);
            var labels = from.EnumerateUntil(to, timeDiffKind);

            IReadOnlyDictionary<DateTime, double> data;

            if (!request.Core.HasValue)
            {
                var entries = await _dbContext.MetricsEntries.AsNoTracking()
                    .Where(x => x.MachineId == request.MachineId)
                    .Where(x => x.ReportDateTime >= from && x.ReportDateTime < to)
                    .OrderBy(x => x.ReportDateTime)
                    .ToArrayAsync(cancellationToken);

                data = entries.GroupedAverage(
                    x => x.ReportDateTime,
                    x => x.Cpu.AveragePercentUsed,
                    timeDiffKind);
            }
            else
            {
                var coreEntries = await _dbContext.CoreUsageEntries.AsNoTracking()
                    .Where(x => x.Entry.MachineId == request.MachineId)
                    .Where(x => x.Entry.ReportDateTime >= from && x.Entry.ReportDateTime < to)
                    .Where(x => x.Ordinal == request.Core)
                    .Select(x => new { x.Entry.ReportDateTime, x.PercentUsed })
                    .OrderBy(x => x.ReportDateTime)
                    .ToArrayAsync(cancellationToken);

                data = coreEntries.GroupedAverage(
                    x => x.ReportDateTime,
                    x => x.PercentUsed,
                    timeDiffKind);
            }
            
            var chartData = new ChartData<double>(labels);
            chartData.Collect(data, 0.0);

            return chartData;
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
