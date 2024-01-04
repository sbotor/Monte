namespace Monte.Services;

public interface IMetricsKeyGenerator
{
    string GenerateKey();
}

public class MetricsKeyGenerator : IMetricsKeyGenerator
{
    public string GenerateKey()
        => Guid.NewGuid().ToString("N");
}
