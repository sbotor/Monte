namespace Monte.Extensions;

public static class AggregationExtensions
{
    public static IReadOnlyDictionary<DateTime, double> GroupedAverage<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, DateTime> keySelector,
        Func<TSource, double> valueSelector,
        DateTimeDiffKind diffKind)
    {
        Func<TSource, DateTime> modifiedKeySelector = diffKind switch
        {
            DateTimeDiffKind.QuarterHours => x => keySelector(x).MatchQuarterHour(),
            DateTimeDiffKind.Days => x => keySelector(x).Date,
            DateTimeDiffKind.Months => x => keySelector(x).BeginningOfTheMonth(),
            _ => throw new InvalidOperationException()
        };

        return source.GroupBy(modifiedKeySelector)
            .ToDictionary(x => x.Key, x => x.Average(valueSelector));
    }
}
