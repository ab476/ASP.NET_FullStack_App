namespace Common.Infrastructure.Time;

public interface ITimeProvider
{
    DateTime LocalNow { get; }
    DateTime UtcNow { get; }
}
