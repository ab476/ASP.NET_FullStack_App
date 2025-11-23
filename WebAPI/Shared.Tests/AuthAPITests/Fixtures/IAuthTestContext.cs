using AuthAPI.Data;

namespace Shared.Tests.AuthAPITests.Fixtures;

public interface IAuthTestContext : IAsyncLifetime
{
    HttpClient Client { get; }
    AuthWebApplicationFactory Factory { get; }
    AuthDbContext CreateDbContext();
    FakeTimeProvider TimeProvider { get; }
}

