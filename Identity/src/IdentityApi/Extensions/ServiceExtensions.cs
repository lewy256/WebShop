using IdentityApi.Entities;
using IdentityApi.Infrastructure;
using IdentityApi.Service;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

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
                    .AllowAnyHeader()));
    }

    public static void ConfigureSwagger(this IServiceCollection services) {
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Identity API",
                Version = "v1",
                Description = "Identity API by lewy256",
                Contact = new OpenApiContact {
                    Name = "lewy256",
                    Url = new Uri("https://github.com/lewy256"),
                }
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement(){{
                new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                new List<string>()
                }
            });
        });
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

    public static void ConfigureProblemDetails(this IServiceCollection services) {
        services.AddProblemDetails(options => {
            options.CustomizeProblemDetails = context => {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });
    }
}