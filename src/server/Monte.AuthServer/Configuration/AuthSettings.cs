namespace Monte.AuthServer.Configuration;

public class AuthSettings
{
    public Uri RedirectUri { get; set; } = null!;
    public bool IsDevelopment { get; set; }
}