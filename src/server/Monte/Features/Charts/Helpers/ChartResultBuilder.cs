namespace Monte.Features.Charts.Helpers;

public interface IChartResultBuilder
{
    IChartResultGroupedBuilder<TSource> Group<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, DateTime> keySelector);
}

public class ChartResultBuilder : IChartResultBuilder
{
    public IChartResultGroupedBuilder<TSource> Group<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, DateTime> keySelector)
        => new ChartResultGroupedBuilder<TSource>(source, keySelector);
}
