namespace Monte.Models;

public class Sorting
{
    public string? Value { get; set; }
    public bool Descending { get; set; }

    public Sorting()
    {
    }

    public Sorting(string? value, bool descending)
    {
        Value = value;
        Descending = descending;
    }
}
