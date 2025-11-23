using Common.Infrastructure.Time;

namespace Shared.Tests.AuthAPITests.Fixtures;

public class FakeTimeProvider(DateTime? dateTime = null) : ITimeProvider
{
    public DateTime Value = dateTime ?? DateTime.UtcNow;
    public DateTime LocalNow => Value;

    public DateTime UtcNow => Value;
}
