namespace AuthAPI.Modules.Auth.Infrastructure;

public interface IDomainEventPublisher
{
    Task PublishAsync(string name, object payload, CancellationToken ct = default);
}
