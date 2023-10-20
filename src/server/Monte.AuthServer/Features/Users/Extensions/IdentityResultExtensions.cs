using Microsoft.AspNetCore.Identity;

namespace Monte.AuthServer.Features.Users.Extensions;

public static class IdentityResultExtensions
{
    public static IReadOnlyDictionary<string, string[]> ToErrorDict(this IEnumerable<IdentityError> errors)
        => errors.GroupBy(x => x.Code)
            .ToDictionary(
                x => x.Key,
                x => x.Select(x => x.Description).ToArray());
}