namespace SecundMemory.Application.Common.Cqrs;

/// <summary>Return type for commands with no meaningful result.</summary>
public readonly struct Unit
{
    public static readonly Unit Value = new();
}
