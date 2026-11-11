using SecundMemory.Domain.Pages;
using SecundMemory.Domain.Pages.Events;

namespace SecundMemory.Domain.UnitTests.Pages;

public class PageTests
{
    [Fact]
    public void Create_RaisesPageCreatedEvent()
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create("Projeto X"));

        var @event = Assert.Single(page.DomainEvents);
        Assert.IsType<PageCreatedEvent>(@event);
    }

    [Fact]
    public void Rename_RaisesPageRenamedEvent_WithOldAndNewTitle()
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create("Old Title"));
        page.ClearDomainEvents();

        page.Rename(PageTitle.Create("New Title"));

        var @event = Assert.Single(page.DomainEvents);
        var renamed = Assert.IsType<PageRenamedEvent>(@event);
        Assert.Equal("Old Title", renamed.OldTitle);
        Assert.Equal("New Title", renamed.NewTitle);
    }

    [Fact]
    public void Rename_ToSameTitle_RaisesNoEvent()
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create("Same"));
        page.ClearDomainEvents();

        page.Rename(PageTitle.Create("Same"));

        Assert.Empty(page.DomainEvents);
    }

    [Theory]
    [InlineData("projeto/subprojeto", "projeto")]
    [InlineData("sem-namespace", null)]
    public void Namespace_IsEverythingBeforeTheLastSlash(string title, string? expectedNamespace)
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create(title));

        Assert.Equal(expectedNamespace, page.Namespace);
    }

    [Fact]
    public void AddAlias_IsCaseInsensitiveAndDeduplicates()
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create("Title"));

        page.AddAlias(PageAlias.Create("Alias"));
        page.AddAlias(PageAlias.Create("alias"));

        Assert.Single(page.Aliases);
    }

    [Fact]
    public void MarkDeleted_SetsIsDeletedAndTimestamp()
    {
        var page = Page.Create(Guid.NewGuid(), PageTitle.Create("Title"));

        page.MarkDeleted();

        Assert.True(page.IsDeleted);
        Assert.NotNull(page.DeletedAtUtc);
    }
}
