namespace AuthAPI.Services.Time;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime LocalNow => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}
