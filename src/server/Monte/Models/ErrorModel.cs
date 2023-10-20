namespace Monte.Models;

public class ErrorModel
{
    public string Key { get; }
    public IReadOnlyCollection<ErrorMessage>? Values { get; }

    public ErrorModel(string key, IEnumerable<ErrorMessage> values)
    {
        Key = key;
        Values = values.ToArray();
    }

    public ErrorModel(string key, string? code, string? message)
    {
        Key = key;
        Values = new ErrorMessage[]
        {
            new(code, message)
        };
    }
}

public record ErrorMessage(string? Code, string? Message);