using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using SecundMemory.Application.Capacity;
using SecundMemory.Application.Notes;
using SecundMemory.Application.ProfileFacts;

namespace SecundMemory.Api.Mcp;

/// <summary>
/// Exposes the second brain to any MCP-compatible AI assistant (Claude, etc.)
/// so facts about the user and their notes can be read as context instead of
/// being re-typed into every new conversation or service.
/// </summary>
[McpServerToolType]
public static class SecondBrainTools
{
    [McpServerTool(Name = "get_profile"), Description(
        "Returns durable facts about the user (preferences, bio, context) so you don't have to ask them to " +
        "re-explain who they are. Optionally filter by category.")]
    public static async Task<string> GetProfile(
        IProfileFactService profileFactService,
        [Description("Optional category filter, e.g. 'preferences', 'bio', 'work'.")] string? category = null)
    {
        var facts = await profileFactService.GetAllAsync(category);
        return JsonSerializer.Serialize(facts);
    }

    [McpServerTool(Name = "search_notes"), Description(
        "Searches the user's personal notes/vault by title or content. Leave query empty to list recent notes.")]
    public static async Task<string> SearchNotes(
        INoteService noteService,
        [Description("Free-text search query, matched against note title and content.")] string? query = null)
    {
        var notes = await noteService.SearchAsync(query);
        return JsonSerializer.Serialize(notes);
    }

    [McpServerTool(Name = "get_capacity_summary"), Description(
        "Returns counts of notes, profile facts and tags currently stored in the second brain.")]
    public static async Task<string> GetCapacitySummary(ICapacityService capacityService)
    {
        var summary = await capacityService.GetSummaryAsync();
        return JsonSerializer.Serialize(summary);
    }
}
