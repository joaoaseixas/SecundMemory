using Microsoft.AspNetCore.SignalR;

namespace SecundMemory.Api.Hubs;

/// <summary>
/// Stub per docs/adr/0004 — real-time collaboration (conflict resolution
/// when two clients edit the same block concurrently) is Fase 3 item #16.
/// For now this only does presence grouping by workspace, so the wiring
/// (MapHub, client connection) exists and is testable ahead of the real
/// collaborative-editing logic.
/// </summary>
public class SyncHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var workspaceId = Context.GetHttpContext()?.Request.Query["workspaceId"].ToString();
        if (!string.IsNullOrEmpty(workspaceId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, workspaceId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>Broadcast-only for now: "a block changed, go re-fetch it" — not an actual CRDT/OT merge.</summary>
    public async Task NotifyBlockChanged(string workspaceId, Guid blockId)
    {
        await Clients.OthersInGroup(workspaceId).SendAsync("blockChanged", blockId);
    }
}
