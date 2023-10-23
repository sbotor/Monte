namespace Monte.AuthServer.Configuration;

public class OidcAppSettings
{
    public ClientCredentialsSettings Agent { get; set; } = new();
    public ClientCredentialsSettings Api { get; set; } = new();
}

public class ClientCredentialsSettings
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;

    public void Validate(string error)
    {
        if (string.IsNullOrEmpty(ClientId)
            || string.IsNullOrEmpty(ClientSecret))
        {
            throw new InvalidOperationException(error);
        }
    }
}
