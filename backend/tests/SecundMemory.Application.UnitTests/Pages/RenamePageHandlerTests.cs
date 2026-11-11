using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Pages.Commands.CreatePage;
using SecundMemory.Application.Features.Pages.Commands.RenamePage;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Pages;

public class RenamePageHandlerTests
{
    [Fact]
    public async Task Handle_RenamesExistingPage()
    {
        var repository = new FakePageRepository();
        var created = await new CreatePageHandler(repository).Handle(new CreatePageCommand(Guid.NewGuid(), "Old"), CancellationToken.None);

        var result = await new RenamePageHandler(repository).Handle(new RenamePageCommand(created.Id, "New"), CancellationToken.None);

        Assert.Equal("New", result.Title);
    }

    [Fact]
    public async Task Handle_UnknownPageId_ThrowsNotFound()
    {
        var sut = new RenamePageHandler(new FakePageRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => sut.Handle(new RenamePageCommand(Guid.NewGuid(), "New"), CancellationToken.None));
    }
}
