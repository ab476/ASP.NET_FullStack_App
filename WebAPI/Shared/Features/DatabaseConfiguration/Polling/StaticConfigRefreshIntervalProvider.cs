namespace Common.Features.DatabaseConfiguration.Polling;
public class StaticConfigRefreshIntervalProvider(TimeSpan interval) : IConfigRefreshInterval
{
    private readonly TimeSpan _interval = interval;

    public TimeSpan Value => _interval;
}
