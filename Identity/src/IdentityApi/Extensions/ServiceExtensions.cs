using IdentityApi.Models;
using IdentityApi.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IdentityService>();


    }

    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options =>
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")));
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<IdentitytContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SQLServer")));
    }

    public static void ConfigureHealthCheck(this IServiceCollection services, IConfiguration configuration) {
        services.AddHealthChecks().AddSqlServer(configuration.GetConnectionString("SQLServer"));
    }

    public static void ConfigureIdentity(this IServiceCollection services) {
        var builder = services.AddIdentity<User, IdentityRole>(o => {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 10;
            o.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentitytContext>()
        .AddDefaultTokenProviders();
    }

}