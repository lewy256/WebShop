using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OneOf.Types;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Service.Configurations;
using ProductApi.Service.Interfaces;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.V1;

public class FileService : IFileService {
    private readonly AzureBlobStorageConfiguration _configuration;
    private readonly ProductContext _productContext;

    public FileService(IOptions<AzureBlobStorageConfiguration> configuration, ProductContext productContext) {
        _configuration = configuration.Value;
        _productContext = productContext;
    }

    public async Task<ImageDeleteResponse> DeleteImageAsync(Guid productId, Guid fileId) {
        var product = await _productContext.Product.Include(i => i.Images).SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var image = product.Images.SingleOrDefault(i => i.Id.Equals(fileId));

        if(image is null) {
            return new NotFoundResponse(fileId, nameof(Image));
        }

        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var blob = container.GetBlobClient(fileId.ToString());

        if(blob is null) {
            return new NotFoundResponse(fileId, nameof(Image));
        }

        await blob.DeleteAsync();

        product.Images.Remove(image);
        await _productContext.SaveChangesAsync();

        return new Success();
    }

    public ImageGetAllResponse GetUrisForImages(List<Image> images) {
        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var uris = new List<string>();

        foreach(var image in images) {
            var fullUri = $"{container.Uri}/{image.Id}";

            uris.Add(fullUri);
        }

        return uris;
    }

    private IEnumerable<string> _allowedContentTypes { get; init; } = [".png", ".jpg"];

    public async Task<ImagesCreateResponse> CreateImagesAsync(Guid productId, Stream fileStream, string contentType) {
        var product = await _productContext.Product.Include(i => i.Images).SingleOrDefaultAsync(p => p.Id.Equals(productId));

        if(product is null) {
            return new NotFoundResponse(productId, nameof(Product));
        }

        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var boundary = GetBoundary(MediaTypeHeaderValue.Parse(contentType));
        var multipartReader = new MultipartReader(boundary, fileStream);
        var section = await multipartReader.ReadNextSectionAsync();

        int fileCount = 0;
        long totalSizeInBytes = 0;
        var notUploadedFiles = new List<string>();
        var uploadedFiles = new List<string>();

        product.Images = new List<Image>();

        while(section is not null) {
            var fileSection = section.AsFileSection();
            if(fileSection is not null) {
                var extension = Path.GetExtension(fileSection.FileName);
                if(!_allowedContentTypes.Contains(extension)) {
                    notUploadedFiles.Add(fileSection.FileName);
                }
                else {
                    var fileId = Guid.NewGuid();
                    var blobClient = container.GetBlobClient(fileId.ToString());

                    await blobClient.UploadAsync(fileSection.FileStream, new BlobHttpHeaders { ContentType = "image/png" });
                    totalSizeInBytes += fileSection.FileStream.Length;

                    uploadedFiles.Add(fileId.ToString());

                    product.Images.Add(new Image() {
                        Id = fileId
                    });

                    fileCount += 1;
                }
            }

            section = await multipartReader.ReadNextSectionAsync();
        }

        await _productContext.SaveChangesAsync();

        return new FileDto {
            TotalFilesUploaded = fileCount,
            TotalSizeUploaded = ConvertSizeToString(totalSizeInBytes),
            FileNames = uploadedFiles,
            NotUploadedFiles = notUploadedFiles
        };
    }

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

