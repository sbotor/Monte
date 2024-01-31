namespace Monte.AuthServer.Configuration;

public class AuthSettings
{
    public Uri RedirectUri { get; set; } = null!;
    public bool IsDevelopment { get; set; }
    public GoogleAuthSettings Google { get; set; } = null!;
}

public class GoogleAuthSettings
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public Uri RedirectUri { get; set; } = null!;
}
