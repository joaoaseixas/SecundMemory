using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SecundMemory.Infrastructure.Persistence.Configurations;

internal static class ValueConverters
{
    public static ValueConverter<List<Guid>, string> GuidListAsJson { get; } = new(
        list => JsonSerializer.Serialize(list, (JsonSerializerOptions?)null),
        json => JsonSerializer.Deserialize<List<Guid>>(json, (JsonSerializerOptions?)null) ?? new List<Guid>());
}
