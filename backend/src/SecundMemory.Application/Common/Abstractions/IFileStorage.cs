namespace SecundMemory.Application.Common.Abstractions;

public interface IFileStorage
{
    /// <returns>The storage key to persist on the Attachment (a relative path or S3 object key).</returns>
    Task<string> UploadAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
