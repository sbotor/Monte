namespace Monte;

public interface IClock
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
}

public class Clock : IClock
{
    public DateTime Now => DateTime.UtcNow;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Today => Now.Date;
}
