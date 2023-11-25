namespace Monte.AuthServer.Configuration;

public class TokenSettings
{
    public LifetimesConfig Lifetimes { get; set; } = new();
    public string SigningKey { get; set; } = null!;
    public Uri Issuer { get; set; } = null!;

    public class LifetimesConfig
    {
        public TimeSpan AuthCode { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan AccessToken { get; set; } = TimeSpan.FromHours(1);
    }
}
