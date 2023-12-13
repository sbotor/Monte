using NSubstitute;

namespace Monte.Tests.Infrastructure.Extensions;

public static class ClockMockExtensions
{
    public static void Setup(this IClock clock, DateTime now)
    {
        clock.Now.Returns(now);
        clock.UtcNow.Returns(now);
        clock.Today.Returns(now.Date);
    }
}
