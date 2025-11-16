namespace Common.Services.Time;

public interface ITimeProvider
{
    DateTime LocalNow { get; }
    DateTime UtcNow { get; }
}
