namespace Monte.Extensions;

public enum DateTimeDiffKind : byte
{
    QuarterHours,
    Days,
    Months
}

public static class DateTimeExtensions
{
    private const int TotalSecondsInQuarter = 900;
    private static readonly int QuarterSecondsThreshold = TotalSecondsInQuarter / 2;
    
    public static IEnumerable<DateTime> EnumerateMinutesUntil(this DateTime from,
        DateTime to,
        int increment = 1)
        => from.EnumerateUntil(to, x => x.AddMinutes(increment));

    public static IEnumerable<DateTime> EnumerateDaysUntil(this DateTime from,
        DateTime to)
        => from.EnumerateUntil(to, x => x.AddDays(1));
    
    public static IEnumerable<DateTime> EnumerateMonthsUntil(this DateTime from,
        DateTime to)
        => from.EnumerateUntil(to, x => x.AddMonths(1));

    public static IEnumerable<DateTime> EnumerateUntil(this DateTime from,
        DateTime to,
        DateTimeDiffKind diffKind)
        => diffKind switch
        {
            DateTimeDiffKind.QuarterHours => from.EnumerateMinutesUntil(to, 15),
            DateTimeDiffKind.Days => from.EnumerateDaysUntil(to),
            DateTimeDiffKind.Months => from.BeginningOfTheMonth().EnumerateMonthsUntil(to),
            _ => throw new InvalidOperationException()
        };

    private static IEnumerable<DateTime> EnumerateUntil(this DateTime from,
        DateTime to,
        Func<DateTime, DateTime> next)
    {
        var dt = from;
        while (dt < to)
        {
            yield return dt;
            dt = next(dt);
        }
    }

    public static DateTime RoundToNextDay(this DateTime dt)
        => dt.Date.AddDays(1);

    public static DateTime MatchQuarterHour(this DateTime dt)
    {
        var totalSeconds = (dt.Minute * 60) + dt.Second;
        
        var remainder = dt.Minute % 15;
        var minutes = totalSeconds % TotalSecondsInQuarter >= QuarterSecondsThreshold
            ? dt.Minute + (15 - remainder)
            : dt.Minute - remainder;

        return new(dt.Year, dt.Month, dt.Day, dt.Hour, minutes, 0, dt.Kind);
    }
    
    public static DateTime BeginningOfTheMonth(this DateTime dt)
        => new(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);

    public static DateTimeDiffKind GetDiffKind(this DateTime left, DateTime right)
    {
        var diff = left - right;

        return Math.Abs(diff.Days) switch
        {
            <= 1 => DateTimeDiffKind.QuarterHours,
            <= 30 => DateTimeDiffKind.Days,
            _ => DateTimeDiffKind.Months
        };
    }
}
