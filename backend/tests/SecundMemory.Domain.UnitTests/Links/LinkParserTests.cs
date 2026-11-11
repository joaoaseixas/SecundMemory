using SecundMemory.Domain.Links;

namespace SecundMemory.Domain.UnitTests.Links;

public class LinkParserTests
{
    [Fact]
    public void ExtractPageLinks_FindsAllDistinctLinks()
    {
        var links = LinkParser.ExtractPageLinks("See [[Page A]] and also [[Page B]], again [[Page A]].");

        Assert.Equal(["Page A", "Page B"], links);
    }

    [Fact]
    public void ExtractPageLinks_WithNoLinks_ReturnsEmpty()
    {
        Assert.Empty(LinkParser.ExtractPageLinks("just plain text"));
    }

    [Fact]
    public void ExtractBlockReferences_FindsValidGuids()
    {
        var id = Guid.NewGuid();

        var refs = LinkParser.ExtractBlockReferences($"embedded block: (({id}))");

        Assert.Equal([id], refs);
    }

    [Fact]
    public void ExtractBlockReferences_IgnoresMalformedGuid()
    {
        Assert.Empty(LinkParser.ExtractBlockReferences("((not-a-guid))"));
    }
}
