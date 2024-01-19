using Monte.Extensions;
using Monte.Features.Charts.Models;

namespace Monte.Features.Charts.Helpers;

public interface IChartResultAggregator
{
    IReadOnlyDictionary<DateTime, double> AggregateValues(
        IEnumerable<RawChartValue> source,
        DateTimeGranularity granularity,
        ChartAggregationType aggregationType);
}

public class ChartResultAggregator : IChartResultAggregator
{
    public IReadOnlyDictionary<DateTime, double> AggregateValues(
        IEnumerable<RawChartValue> source,
        DateTimeGranularity granularity,
        ChartAggregationType aggregationType)
    {
        Func<IEnumerable<RawChartValue>, IEnumerable<IGrouping<DateTime, RawChartValue>>> grouping = granularity switch
        {
            DateTimeGranularity.QuarterHours => AggregationHelpers.GroupByQuarterHour,
            DateTimeGranularity.Days => AggregationHelpers.GroupByDate,
            DateTimeGranularity.Months => AggregationHelpers.GroupByMonth,
            _ => throw new InvalidOperationException()
        };

        Func<IEnumerable<RawChartValue>, double> aggregation = aggregationType switch
        {
            ChartAggregationType.Avg => AggregationHelpers.CalcAverage,
            ChartAggregationType.Min => AggregationHelpers.CalcMin,
            ChartAggregationType.Max => AggregationHelpers.CalcMax,
            _ => throw new InvalidOperationException()
        };

        return grouping(source).ToDictionary(
                x => x.Key,
                x => aggregation(x));
    }
}
