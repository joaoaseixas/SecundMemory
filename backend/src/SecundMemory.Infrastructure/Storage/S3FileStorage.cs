using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Infrastructure.Storage;

public class S3StorageOptions
{
    public const string SectionName = "S3Storage";
    public string BucketName { get; init; } = string.Empty;
    public string Region { get; init; } = "us-east-1";
}

public class S3FileStorage(IAmazonS3 s3Client, IOptions<S3StorageOptions> options) : IFileStorage
{
    private readonly string _bucketName = options.Value.BucketName;

    public async Task<string> UploadAsync(string fileName, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var key = $"{Guid.NewGuid()}-{fileName}";

        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType,
        }, cancellationToken);

        return key;
    }

    public async Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var response = await s3Client.GetObjectAsync(_bucketName, storageKey, cancellationToken);
        return response.ResponseStream;
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default) =>
        await s3Client.DeleteObjectAsync(_bucketName, storageKey, cancellationToken);
}
