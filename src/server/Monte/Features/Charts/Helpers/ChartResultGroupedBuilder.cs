using Monte.Extensions;

namespace Monte.Features.Charts.Helpers;

public interface IChartResultGroupedBuilder<out TSource>
{
    IChartResultGroupedBuilder<TSource> WithGranularity(DateTimeGranularity granularity);

    IReadOnlyDictionary<DateTime, double> Apply(
        Func<TSource, double> valueSelector,
        ChartAggregationType aggregationType);
}

public class ChartResultGroupedBuilder<TSource>
    : IChartResultGroupedBuilder<TSource>
{
    private readonly IEnumerable<TSource> _source;
    private readonly Func<TSource, DateTime> _keySelector;

    private DateTimeGranularity _dtDiff = DateTimeGranularity.Days;

    public ChartResultGroupedBuilder(
        IEnumerable<TSource> source,
        Func<TSource, DateTime> keySelector)
    {
        _source = source;
        _keySelector = keySelector;
    }

    public IChartResultGroupedBuilder<TSource> WithGranularity(DateTimeGranularity granularity)
    {
        _dtDiff = granularity;
        return this;
    }

    public IReadOnlyDictionary<DateTime, double> Apply(
        Func<TSource, double> valueSelector,
        ChartAggregationType aggregationType)
    {
        Func<IGrouping<DateTime, TSource>, double> aggregationOp = aggregationType switch
        {
            ChartAggregationType.Avg => x => x.Average(valueSelector),
            ChartAggregationType.Min => x => x.Min(valueSelector),
            ChartAggregationType.Max => x => x.Max(valueSelector),
            _ => throw new InvalidOperationException()
        };

        return Apply(aggregationOp);
    }

    private IReadOnlyDictionary<DateTime, TValues> Apply<TValues>(
        Func<IGrouping<DateTime, TSource>, TValues> aggregation)
    {
        Func<TSource, DateTime> modifiedKeySelector = _dtDiff switch
        {
            DateTimeGranularity.QuarterHours => x => _keySelector(x).MatchQuarterHour(),
            DateTimeGranularity.Days => x => _keySelector(x).Date,
            DateTimeGranularity.Months => x => _keySelector(x).BeginningOfTheMonth(),
            _ => throw new InvalidOperationException()
        };

        return _source.GroupBy(modifiedKeySelector)
            .ToDictionary(x => x.Key, aggregation);
    }
}
