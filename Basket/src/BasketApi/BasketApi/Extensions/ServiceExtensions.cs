using BasketApi.Services;

namespace BasketApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<BasketService>();

        services.AddSingleton<PublisherService>();

    }
}