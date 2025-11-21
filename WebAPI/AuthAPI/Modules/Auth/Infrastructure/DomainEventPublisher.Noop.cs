namespace AuthAPI.Modules.Auth.Infrastructure;

public class NoopDomainEventPublisher : IDomainEventPublisher
{
    public Task PublishAsync(string name, object payload, CancellationToken ct = default) => Task.CompletedTask;
}
