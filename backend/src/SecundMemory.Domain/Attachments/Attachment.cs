using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Attachments;

public class Attachment : AggregateRoot, ISoftDeletable
{
    public Guid WorkspaceId { get; private init; }
    public Guid? BlockId { get; private set; }
    public string FileName { get; private init; } = string.Empty;
    public string ContentType { get; private init; } = string.Empty;
    public long SizeBytes { get; private init; }

    /// <summary>Key used to retrieve the file from IFileStorage — a relative path or S3 object key.</summary>
    public string StorageKey { get; private init; } = string.Empty;

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private Attachment() { }

    public static Attachment Create(Guid workspaceId, string fileName, string contentType, long sizeBytes, string storageKey)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("Storage key cannot be empty.", nameof(storageKey));

        return new Attachment
        {
            WorkspaceId = workspaceId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
        };
    }

    public void AttachToBlock(Guid blockId) => BlockId = blockId;

    public void DetachFromBlock() => BlockId = null;

    public void MarkDeleted()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}
