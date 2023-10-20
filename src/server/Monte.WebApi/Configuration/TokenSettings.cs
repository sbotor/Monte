namespace Monte.WebApi.Configuration;

public class TokenSettings
{
    public string Issuer { get; set; } = null!;
    public string Key { get; set; } = null!;
}