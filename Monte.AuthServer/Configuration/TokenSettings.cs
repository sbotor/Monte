namespace Monte.AuthServer.Configuration;

public class TokenSettings
{
    public LifetimesConfig Lifetimes { get; set; } = new();

    public class LifetimesConfig
    {
        public TimeSpan AuthCode { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan AccessToken { get; set; } = TimeSpan.FromHours(1);
    }
}