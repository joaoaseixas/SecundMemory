using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Infrastructure.Persistence;

namespace SecundMemory.Infrastructure.Search;

/// <summary>
/// Raw SQL rather than LINQ: Page.Title/Block.Content are mapped through a
/// value converter (PageTitle/BlockContent &lt;-&gt; string), and EF Core
/// cannot translate member access into the pre-conversion CLR type inside a
/// query — only the whole converted property. ts_rank's scoring expression
/// also just reads more naturally as SQL than as a LINQ-translated function
/// chain.
/// </summary>
public class PostgresFullTextSearchService(ApplicationDbContext db) : ISearchService
{
    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        Guid workspaceId, string query, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];

        var pageRows = await db.Database.SqlQuery<PageSearchRow>($"""
            SELECT "Id", "Title",
                   ts_rank(to_tsvector('english', "Title"), websearch_to_tsquery('english', {query})) AS "Rank"
            FROM "Pages"
            WHERE "WorkspaceId" = {workspaceId}
              AND to_tsvector('english', "Title") @@ websearch_to_tsquery('english', {query})
            ORDER BY "Rank" DESC
            LIMIT {limit}
            """).ToListAsync(cancellationToken);

        var blockRows = await db.Database.SqlQuery<BlockSearchRow>($"""
            SELECT b."Id", b."PageId", p."Title", LEFT(b."Content", 160) AS "Snippet",
                   ts_rank(to_tsvector('english', b."Content"), websearch_to_tsquery('english', {query})) AS "Rank"
            FROM "Blocks" b
            INNER JOIN "Pages" p ON p."Id" = b."PageId"
            WHERE p."WorkspaceId" = {workspaceId}
              AND to_tsvector('english', b."Content") @@ websearch_to_tsquery('english', {query})
            ORDER BY "Rank" DESC
            LIMIT {limit}
            """).ToListAsync(cancellationToken);

        var pageResults = pageRows.Select(r => new SearchResult(r.Id, SearchResultKind.Page, r.Id, r.Title, r.Title, r.Rank));
        var blockResults = blockRows.Select(r => new SearchResult(r.Id, SearchResultKind.Block, r.PageId, r.Title, r.Snippet, r.Rank));

        return pageResults.Concat(blockResults).OrderByDescending(r => r.Rank).Take(limit).ToList();
    }

    private record PageSearchRow(Guid Id, string Title, float Rank);
    private record BlockSearchRow(Guid Id, Guid PageId, string Title, string Snippet, float Rank);
}
