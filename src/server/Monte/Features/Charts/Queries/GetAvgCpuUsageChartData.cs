using Microsoft.EntityFrameworkCore;
using Monte.Cqrs;
using Monte.Extensions;
using Monte.Features.Charts.Models;
using Monte.Features.Metrics;

namespace Monte.Features.Charts.Queries;

public static class GetAvgCpuUsageChartData
{
    public record Query(Guid MachineId, DateTime? From, DateTime? To)
        : IQuery<ChartData<double>>;

    internal class Handler : IQueryHandler<Query, ChartData<double>>
    {
        private readonly MonteDbContext _dbContext;
        private readonly IClock _clock;

        public Handler(MonteDbContext dbContext, IClock clock)
        {
            _dbContext = dbContext;
            _clock = clock;
        }

        public async Task<ChartData<double>> Handle(Query request, CancellationToken cancellationToken)
        {
            var today = _clock.Today;

            var from = (request.From?.ToUniversalTime() ?? today)
                .TruncateSeconds();
            var to = (request.To?.ToUniversalTime() ?? today.AddDays(1).AddMinutes(-1))
                .TruncateSeconds().ToUniversalTime();

            var results = await _dbContext.MetricsEntries.AsNoTracking()
                .Where(x => x.MachineId == request.MachineId)
                .Where(x => x.ReportDateTime >= from && x.ReportDateTime < to)
                .OrderBy(x => x.ReportDateTime)
                .ToArrayAsync(cancellationToken);

            var timeDiffKind = to.GetDiffKind(from);

            Func<MetricsEntry, DateTime> keySelector;
            IEnumerable<DateTime> labels;

            switch (timeDiffKind)
            {
                case DateTimeDiffKind.Quarters:
                    keySelector = x => x.ReportDateTime.MatchQuarter();
                    labels = from.EnumerateMinutesUntil(to, 15);
                    break;
                case DateTimeDiffKind.Days:
                    keySelector = x => x.ReportDateTime.Date;
                    labels = from.EnumerateDaysUntil(to);
                    break;
                case DateTimeDiffKind.Months:
                    keySelector = x => x.ReportDateTime.BeginningOfTheMonth();
                    labels = from.BeginningOfTheMonth().EnumerateMonthsUntil(to);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var data = results.GroupBy(keySelector)
                .ToDictionary(x => x.Key,
                    x => x.Average(y => y.Cpu.AveragePercentUsed));
            
            var chartData = new ChartData<double>(labels);
            var i = 0;

            foreach (var date in chartData.Labels)
            {
                chartData.Values[i] = data.GetValueOrDefault(date, 0d);
                i++;
            }

            return chartData;
        }
    }
}
