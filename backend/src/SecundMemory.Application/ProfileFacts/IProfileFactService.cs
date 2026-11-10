namespace SecundMemory.Application.ProfileFacts;

public interface IProfileFactService
{
    Task<IReadOnlyList<ProfileFactDto>> GetAllAsync(string? category, CancellationToken cancellationToken = default);
    Task<ProfileFactDto> UpsertAsync(UpsertProfileFactRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
