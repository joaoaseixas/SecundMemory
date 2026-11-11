using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;
using SecundMemory.Application.Features.Pages.Queries.GetBacklinks;
using SecundMemory.Application.Features.Search.Queries.SearchAll;

namespace SecundMemory.Api.Mcp;

/// <summary>
/// Exposes the second brain (now Page/Block, not the old Note/ProfileFact
/// model) to any MCP-compatible AI assistant. Tools take workspaceId
/// explicitly rather than relying on the HTTP auth context, since MCP
/// transports don't uniformly carry the same ClaimsPrincipal a controller
/// request does.
/// </summary>
[McpServerToolType]
public static class SecondBrainTools
{
    [McpServerTool(Name = "search_second_brain"), Description(
        "Full-text search over the user's pages and blocks. Returns ranked matches with a short snippet.")]
    public static async Task<string> SearchSecondBrain(
        IDispatcher dispatcher,
        [Description("The workspace to search in.")] Guid workspaceId,
        [Description("Free-text search query.")] string query)
    {
        var results = await dispatcher.Send(new SearchAllQuery(workspaceId, query));
        return JsonSerializer.Serialize(results);
    }

    [McpServerTool(Name = "get_page_backlinks"), Description(
        "Lists every block elsewhere in the second brain that links to the given page — i.e. its backlinks.")]
    public static async Task<string> GetPageBacklinks(
        IDispatcher dispatcher,
        [Description("The page to find backlinks for.")] Guid pageId)
    {
        var backlinks = await dispatcher.Send(new GetBacklinksQuery(pageId));
        return JsonSerializer.Serialize(backlinks);
    }

    [McpServerTool(Name = "open_today_journal"), Description(
        "Opens (creating if needed) today's journal page, so notes can be appended to it as blocks.")]
    public static async Task<string> OpenTodayJournal(
        IDispatcher dispatcher,
        [Description("The workspace whose journal to open.")] Guid workspaceId)
    {
        var journal = await dispatcher.Send(new OpenTodayJournalCommand(workspaceId));
        return JsonSerializer.Serialize(journal);
    }
}
