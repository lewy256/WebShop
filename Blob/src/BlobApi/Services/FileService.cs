using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobApi.Configurations;
using BlobApi.Interfaces;
using OneOf.Types;

namespace BlobApi.Services;
public class FileService : IFileService {
    private readonly AzureBlobStorageConfiguration _configuration;

    public FileService(IConfiguration configuration) {
        var azureBlobStorageConfiguration = new AzureBlobStorageConfiguration();
        configuration.Bind(azureBlobStorageConfiguration.Section, azureBlobStorageConfiguration);
        _configuration = azureBlobStorageConfiguration;
    }

    public async Task<ImageGetResponse> GetImageByNameAsync(string imageName) {
        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var blob = container.GetBlobClient(imageName);

        if(blob is null) {
            return new NotFound();
        }

        return blob.Uri.ToString() + ".png";
    }

    public async Task<ImageCreateResponse> CreateImageAsync(Stream fileStream) {
        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var imageName = Guid.NewGuid().ToString();

        var blob = container.GetBlobClient(imageName);

        await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/png" });

        return imageName;
    }

    public async Task<ImageDeleteResponse> DeleteImageAsync(string imageName) {
        var container = new BlobContainerClient(_configuration.ConnectionString, _configuration.Container);

        var blob = container.GetBlobClient(imageName);

        if(blob is null) {
            return new NotFound();
        }

        await blob.DeleteAsync();

        return new Success();

    }
}
