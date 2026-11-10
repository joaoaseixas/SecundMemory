namespace SecundMemory.Application.Capacity;

public record CapacitySummaryDto(
    int NoteCount,
    int ProfileFactCount,
    int TagCount,
    DateTime LastUpdatedUtc);
