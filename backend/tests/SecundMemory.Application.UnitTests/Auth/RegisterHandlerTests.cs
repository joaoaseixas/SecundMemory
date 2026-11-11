using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Auth.Commands.Register;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Auth;

public class RegisterHandlerTests
{
    [Fact]
    public async Task Handle_CreatesUserAndOwnWorkspace_AndReturnsToken()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new RegisterHandler(db, new FakePasswordHasher(), new FakeJwtTokenService());

        var result = await sut.Handle(new RegisterCommand("new@user.com", "New User", "password123"), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.NotEqual(Guid.Empty, result.WorkspaceId);
        Assert.False(string.IsNullOrEmpty(result.AccessToken));
    }

    [Fact]
    public async Task Handle_DuplicateEmail_Throws()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new RegisterHandler(db, new FakePasswordHasher(), new FakeJwtTokenService());
        await sut.Handle(new RegisterCommand("dup@user.com", "First", "password123"), CancellationToken.None);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<ValidationException>(
            () => sut.Handle(new RegisterCommand("dup@user.com", "Second", "password123"), CancellationToken.None));
    }
}
