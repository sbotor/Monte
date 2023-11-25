namespace Monte.AuthServer.Configuration;

public class OidcAppSettings
{
    public ClientCredentialsSettings Agent { get; set; } = new();
    public ClientCredentialsSettings Client { get; set; } = new();
}

public class ClientCredentialsSettings
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = string.Empty;

    public void Validate(string error, bool skipClientSecret = false)
    {
        if (string.IsNullOrEmpty(ClientId)
            || (!skipClientSecret && string.IsNullOrEmpty(ClientSecret)))
        {
            throw new InvalidOperationException(error);
        }
    }
}
