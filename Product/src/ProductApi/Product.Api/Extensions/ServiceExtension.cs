using Microsoft.EntityFrameworkCore;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Service;

namespace ProductApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IProductService, ProductService>();
    }

    public static void ConfigureCosmosDB(this IServiceCollection services, IConfigurationSection configurationSection) {
        services.AddDbContext<ProductContext>(options =>
            options.UseCosmos(
                configurationSection["AccountEndpoint"],
                configurationSection["AccountKey"],
                configurationSection["DatabaseName"])
        );
    }

    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options =>
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")));
    }
}