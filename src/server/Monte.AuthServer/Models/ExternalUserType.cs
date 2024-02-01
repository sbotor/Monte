namespace Monte.AuthServer.Models;

public enum ExternalUserType
{
    Google
}

public static class ExternalUserTypeExtensions
{
    public static string FormatUsername(this ExternalUserType type, string username)
        => type switch
        {
            ExternalUserType.Google => $"google-{username}",
            _ => throw new InvalidOperationException()
        };
}
