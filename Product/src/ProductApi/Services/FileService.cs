using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using MassTransit.Initializers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OneOf;
using OneOf.Types;
using ProductApi.Entities;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Configurations.AppSettings;
using ProductApi.Shared.FilesDtos;
using ProductApi.Shared.Responses;
using Serilog;
using File = ProductApi.Entities.File;

namespace FileApi.Services;

public interface IFileService {
    Task<FilesCreateResponse> UploadFilesAsync(Guid productId, FileParameters fileParameters);
    Task<FilesDeleteResponse> DeleteFilesAsync(Guid productId, IEnumerable<Guid> fileIds);
}

public class FileService : IFileService {
    private readonly AzureBlobStorageConfiguration _configuration;
    private readonly ProductContext _productContext;

    public FileService(IOptions<AzureBlobStorageConfiguration> configuration, ProductContext productContext) {
        _configuration = configuration.Value;
        _productContext = productContext;
    }

    public async Task<FilesCreateResponse> UploadFilesAsync(Guid productId, FileParameters fileParameters) {
        var product = await _productContext.Product.SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var boundary = GetBoundary(MediaTypeHeaderValue.Parse(fileParameters.Request.ContentType));
        var multipartReader = new MultipartReader(boundary, fileParameters.Context.Request.Body);
        var section = await multipartReader.ReadNextSectionAsync();

        int fileCount = 0;
        long totalSizeInBytes = 0;
        var notUploadedFileIds = new List<string>();
        var uploadedFileIds = new List<string>();

        while(section is not null) {
            var fileSection = section.AsFileSection();
            if(fileSection is not null) {
                var extension = Path.GetExtension(fileSection.FileName);
                if(!_allowedContentTypes.Contains(extension)) {
                    notUploadedFileIds.Add(fileSection.FileName);
                }
                else {
                    var fileId = Guid.NewGuid();
                    var blobClient = container.GetBlobClient(fileId.ToString());

                    await blobClient.UploadAsync(fileSection.FileStream, new BlobHttpHeaders { ContentType = "image/png" });
                    totalSizeInBytes += fileSection.FileStream.Length;

                    uploadedFileIds.Add(fileId.ToString());

                    fileCount += 1;
                }
            }

            section = await multipartReader.ReadNextSectionAsync();
        }

        if(product.Files is null) {
            product.Files = new List<File>();
        }

        var files = GetFileUris(uploadedFileIds);

        product.Files.AddRange(files);

        await _productContext.SaveChangesAsync();

        return new FileDto {
            TotalFilesUploaded = fileCount,
            TotalSizeUploaded = ConvertSizeToString(totalSizeInBytes),
            FileNames = uploadedFileIds,
            NotUploadedFiles = notUploadedFileIds
        };
    }

    public async Task<FilesDeleteResponse> DeleteFilesAsync(Guid productId, IEnumerable<Guid> fileIds) {
        var product = await _productContext.Product.SingleOrDefaultAsync(p => p.Id.Equals(productId));
        var ids = fileIds.ToList();

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var files = product.Files.Where(f => fileIds.Contains(f.Id)).ToList();

        if(files.Count != ids.Count) {
            return new NotFoundResponse("Not all given files exist.");
        }

        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var batchClient = container.GetBlobBatchClient();

        var blobs = files.Select(f => new Uri(f.URI));

        try {
            await batchClient.DeleteBlobsAsync(blobs);
        }
        catch(Exception exception) {
            Log.Error(exception, "Exception occurred: {Message}", exception.Message);
            return new NotFoundResponse("The blobs do not exist.");
        }

        product.Files.RemoveAll(f => files.Contains(f));

        await _productContext.SaveChangesAsync();

        return new Success();
    }


    private IEnumerable<File> GetFileUris(List<string> ids) {
        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var files = new List<File>();

        foreach(var id in ids) {
            var completeUri = $"{container.Uri}/{id}";

            files.Add(new File() {
                Id = new Guid(id),
                URI = completeUri
            });
        }

        return files;
    }


    private IEnumerable<string> _allowedContentTypes { get; init; } = [".png", ".jpg"];


    private string ConvertSizeToString(long bytes) {
        var fileSize = new decimal(bytes);
        var kilobyte = new decimal(1024);
        var megabyte = new decimal(1024 * 1024);
        var gigabyte = new decimal(1024 * 1024 * 1024);

        return fileSize switch {
            _ when fileSize < kilobyte => "Less then 1KB",
            _ when fileSize < megabyte =>
                $"{Math.Round(fileSize / kilobyte, fileSize < 10 * kilobyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}KB",
            _ when fileSize < gigabyte =>
                $"{Math.Round(fileSize / megabyte, fileSize < 10 * megabyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}MB",
            _ when fileSize >= gigabyte =>
                $"{Math.Round(fileSize / gigabyte, fileSize < 10 * gigabyte ? 2 : 1, MidpointRounding.AwayFromZero):##,###.##}GB",
            _ => "n/a"
        };
    }

    private string GetBoundary(MediaTypeHeaderValue contentType) {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        if(string.IsNullOrWhiteSpace(boundary)) {
            throw new InvalidDataException("Missing content-type boundary.");
        }

        return boundary;
    }

}

[GenerateOneOf]
public partial class FilesCreateResponse : OneOfBase<FileDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class FilesDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}
