using BlobApi.Interfaces;
using BlobApi.Services;

namespace BlobApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IFileService, FileService>();
    }
}
