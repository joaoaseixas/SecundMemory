namespace SecundMemory.Application.Common.Models;

public record PaginationQuery(int Page = 1, int PageSize = 20)
{
    public int Skip => (Page - 1) * PageSize;
}
