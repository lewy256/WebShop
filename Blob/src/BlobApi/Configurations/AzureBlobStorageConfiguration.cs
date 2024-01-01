namespace BlobApi.Configurations;

public class AzureBlobStorageConfiguration {
    public string Section { get; set; } = "AzureBlobStorage";
    public string ConnectionString { get; set; }
    public string Container { get; set; }
}
