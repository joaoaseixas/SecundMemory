using Microsoft.Extensions.Options;
using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Infrastructure.Storage;

public class LocalFileStorageOptions
{
    public const string SectionName = "LocalFileStorage";
    public string RootPath { get; init; } = "App_Data/attachments";
}

/// <summary>Default for local dev / a single-VPS deployment. Swap to S3FileStorage for anything multi-instance.</summary>
public class LocalFileStorage(IOptions<LocalFileStorageOptions> options) : IFileStorage
{
    private readonly string _rootPath = options.Value.RootPath;

    public async Task<string> UploadAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_rootPath);
        var storageKey = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        await using var fileStream = File.Create(Path.Combine(_rootPath, storageKey));
        await content.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(File.OpenRead(Path.Combine(_rootPath, storageKey)));

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
