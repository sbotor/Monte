namespace Monte.Models;

public class Sorting<TSorting>
    where TSorting : struct, Enum
{
    public TSorting? Value { get; set; }
    public bool Descending { get; set; }

    public Sorting()
    {
    }

    public Sorting(TSorting? value, bool descending)
    {
        Value = value;
        Descending = descending;
    }
}
