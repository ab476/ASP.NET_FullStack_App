using AuthAPI.Data;
using AuthAPI.Data.Models;
using AuthAPI.Modules.Auth.Repositories;
using Shared.Tests.AuthAPITests.Builders;

namespace Shared.Tests.AuthAPITests.Repositories;

[Collection(TestConstants.Auth)]
public class RefreshTokenRepositoryTests(AuthTestContextFixture _fixture)
{
    private readonly FakeTimeProvider _fakeClock = _fixture.TimeProvider;

    private RefreshTokenRepository CreateRepository(AuthDbContext db)
        => new(db, _fakeClock);

    private TUser _user = null!;
    private async Task<TUser> EnsureUserExistsAsync(AuthDbContext db)
    {
        if(_user is not null && await db.TUsers.AnyAsync(u => u.Id == _user.Id))
            return _user;

        var user = new TUserBuilder()
            .WithId(Guid.NewGuid())
            .WithUserName("testuser@example.com")
            .Build();

        await db.TUsers.AddAsync(user);
        await db.SaveChangesAsync();

        return _user = user;
    }

    // ------------ GetByHashAsync ------------

    [Fact]
    public async Task GetByHashAsync_ReturnsToken_WhenExists()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var token = new TRefreshTokenBuilder()
            .WithTokenHash("abc123")
            .Build();

        db.TRefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var result = await repo.GetByHashAsync("abc123");

        result.Should().NotBeNull();
        result!.TokenHash.Should().Be("abc123");
    }

    [Fact]
    public async Task GetByHashAsync_ReturnsNull_WhenNotFound()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var result = await repo.GetByHashAsync("missing");

        result.Should().BeNull();
    }

    // ------------ GetActiveForUserAsync ------------

    [Fact]
    public async Task GetActiveForUserAsync_ReturnsOnlyActiveTokens()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var user = await EnsureUserExistsAsync(db);

        var active = new TRefreshTokenBuilder()
            .WithUser(user)
            .WithTokenHash("active")
            .WithRevoked(false)
            .Build();

        var revoked = new TRefreshTokenBuilder()
            .WithUser(user)
            .WithTokenHash("revoked")
            .WithRevoked(true)
            .Build();


        db.TRefreshTokens.AddRange(active, revoked);
        await db.SaveChangesAsync();

        var result = await repo.GetActiveForUserAsync(user.Id);

        result.Should().ContainSingle()
              .Which.TokenHash.Should().Be("active");
    }

    // ------------ AddAsync & Save ------------

    [Fact]
    public async Task AddAsync_AddsTokenToDb()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var token = new TRefreshTokenBuilder()
            .WithTokenHash("newtoken")
            .Build();

        await repo.AddAsync(token);
        await repo.SaveChangesAsync();

        var exists = await db.TRefreshTokens
            .AsNoTracking()
            .AnyAsync(t => t.TokenHash == "newtoken");

        exists.Should().BeTrue();
    }

    // ------------ RevokeAsync (by Id) ------------

    [Fact]
    public async Task RevokeAsync_SetsRevokedFields()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var tokenId = Guid.NewGuid();

        var token = new TRefreshTokenBuilder()
            .WithId(tokenId)
            .WithTokenHash("hash")
            .Build();

        db.TRefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var result = await repo.RevokeAsync(tokenId, "test-reason");

        result.Should().BeTrue();

        var updated = await db.TRefreshTokens
            .Where(t => t.Id == tokenId)
            .Select(t => new { t.Revoked, t.RevokedAt, t.RevokedReason })
            .FirstAsync();

        updated.Revoked.Should().BeTrue();
        updated.RevokedAt.Should().BeCloseTo(_fakeClock.UtcNow, TimeSpan.FromMilliseconds(1));
        updated.RevokedReason.Should().Be("test-reason");
    }

    // ------------ RevokeByHashAsync ------------

    [Fact]
    public async Task RevokeByHashAsync_RevokesToken()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var tokenId = Guid.NewGuid();

        var token = new TRefreshTokenBuilder()
            .WithId(tokenId)
            .WithTokenHash("hash123")
            .Build();

        db.TRefreshTokens.Add(token);
        await db.SaveChangesAsync();

        var result = await repo.RevokeByHashAsync("hash123");

        result.Should().BeTrue();

        var revoked = await db.TRefreshTokens
            .Where(t => t.Id == tokenId)
            .Select(t => t.Revoked)
            .FirstAsync();

        revoked.Should().BeTrue();
    }

    // ------------ RevokeAllForUserAsync ------------

    [Fact]
    public async Task RevokeAllForUserAsync_RevokesOnlyActiveTokens()
    {
        using var db = _fixture.CreateDbContext();
        var repo = CreateRepository(db);

        var user = await EnsureUserExistsAsync(db);

        var t1 = new TRefreshTokenBuilder()
            .WithUser(user)
            .WithTokenHash("to-be-revoked")
            .WithRevoked(false)
            .Build();

        var t2 = new TRefreshTokenBuilder()
            .WithUser(user)
            .WithTokenHash("already-revoked")
            .WithRevoked(true)
            .Build();

        db.TRefreshTokens.AddRange(t1, t2);
        await db.SaveChangesAsync();

        var count = await repo.RevokeAllForUserAsync(user.Id, "mass-revoke");

        count.Should().Be(1);

        var statusList = await db.TRefreshTokens
            .AsNoTracking()
            .Where(t => t.UserId == user.Id)
            .Select(t => t.Revoked)
            .ToListAsync();

        statusList.Should().OnlyContain(r => r == true);
    }
}
