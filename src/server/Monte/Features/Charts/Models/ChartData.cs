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
}
