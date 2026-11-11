using SecundMemory.Application.Features.Auth.Commands.Login;
using SecundMemory.Application.Features.Auth.Commands.Register;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Auth;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_CorrectPassword_ReturnsToken()
    {
        using var db = TestApplicationDbContext.Create();
        var hasher = new FakePasswordHasher();
        var jwt = new FakeJwtTokenService();
        await new RegisterHandler(db, hasher, jwt).Handle(new RegisterCommand("user@test.com", "User", "correct-password"), CancellationToken.None);
        await db.SaveChangesAsync();

        var result = await new LoginHandler(db, hasher, jwt).Handle(new LoginCommand("user@test.com", "correct-password"), CancellationToken.None);

        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        using var db = TestApplicationDbContext.Create();
        var hasher = new FakePasswordHasher();
        var jwt = new FakeJwtTokenService();
        await new RegisterHandler(db, hasher, jwt).Handle(new RegisterCommand("user2@test.com", "User", "correct-password"), CancellationToken.None);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => new LoginHandler(db, hasher, jwt).Handle(new LoginCommand("user2@test.com", "wrong-password"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorized()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new LoginHandler(db, new FakePasswordHasher(), new FakeJwtTokenService());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.Handle(new LoginCommand("nobody@test.com", "whatever"), CancellationToken.None));
    }
}
