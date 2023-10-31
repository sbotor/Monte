namespace Monte.WebApi.Configuration;

public class TokenSettings
{
    public Uri Issuer { get; set; } = null!;
    public string Key { get; set; } = null!;
}
