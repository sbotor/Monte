namespace Monte.Features.Charts.Models;

public class ChartData<T>
{
    public DateTime[] Labels { get; }
    public T[] Values { get; }

    public ChartData(IEnumerable<DateTime> labels)
    {
        Labels = labels.ToArray();
        Values = new T[Labels.Length];
    }

    public void Collect(IReadOnlyDictionary<DateTime, T> values, T defaultValue)
    {
        var i = 0;

        foreach (var date in Labels)
        {
            Values[i] = values.GetValueOrDefault(date, defaultValue);
            i++;
        }
    }
}
