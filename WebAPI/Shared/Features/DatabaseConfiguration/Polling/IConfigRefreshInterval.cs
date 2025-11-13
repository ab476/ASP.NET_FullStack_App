namespace Common.Features.DatabaseConfiguration.Polling;
public interface IConfigRefreshInterval
{
    TimeSpan Value { get; }
}

