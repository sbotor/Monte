using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Monte.AuthServer.Features.Users.Extensions;

public class ErrorDictionary
{
    private readonly IReadOnlyDictionary<string, string[]> _dict;

    private string? _serialized;
    
    public ErrorDictionary(IReadOnlyDictionary<string, string[]> dict)
    {
        _dict = dict;
    }

    public override string ToString()
    {
        _serialized ??= JsonSerializer.Serialize(_dict);
        return _serialized;
    }

    public static implicit operator string(ErrorDictionary dict)
        => dict.ToString();
}

public static class IdentityResultExtensions
{
    public static ErrorDictionary ToErrorDictionary(this IEnumerable<IdentityError> errors)
    {
        var dict = errors.GroupBy(x => x.Code)
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => y.Description).ToArray());
        
        return new(dict);
    }
}
