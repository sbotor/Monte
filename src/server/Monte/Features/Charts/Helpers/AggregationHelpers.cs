using Monte.Extensions;
using Monte.Features.Charts.Models;

namespace Monte.Features.Charts.Helpers;

internal static class AggregationHelpers
{
    public static double CalcAverage(this IEnumerable<RawChartValue> source)
        => source.Average(x => x.Value);
    
    public static double CalcMin(this IEnumerable<RawChartValue> source)
        => source.Min(x => x.Value);
    
    public static double CalcMax(this IEnumerable<RawChartValue> source)
        => source.Max(x => x.Value);

    public static IEnumerable<IGrouping<DateTime, RawChartValue>> GroupByQuarterHour(
        this IEnumerable<RawChartValue> source)
        => source.GroupBy(x => x.ReportDateTime.MatchQuarterHour());
    
    public static IEnumerable<IGrouping<DateTime, RawChartValue>> GroupByDate(
        this IEnumerable<RawChartValue> source)
        => source.GroupBy(x => x.ReportDateTime.Date);
    
    public static IEnumerable<IGrouping<DateTime, RawChartValue>> GroupByMonth(
        this IEnumerable<RawChartValue> source)
        => source.GroupBy(x => x.ReportDateTime.BeginningOfTheMonth());
}
