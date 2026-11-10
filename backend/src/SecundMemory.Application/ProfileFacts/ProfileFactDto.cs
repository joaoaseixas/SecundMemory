namespace SecundMemory.Application.ProfileFacts;

public record ProfileFactDto(Guid Id, string Category, string Key, string Value, DateTime UpdatedUtc);

public record UpsertProfileFactRequest(string Category, string Key, string Value);
