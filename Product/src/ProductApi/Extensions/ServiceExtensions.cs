using Asp.Versioning;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Contracts.Roles;
using FileApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.Configurations.AppSettings;
using ProductApi.Infrastructure.Filters;
using ProductApi.Infrastructure.Utility;
using ProductApi.Services;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ProductApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPriceHistoryService, PriceHistoryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IFileService, FileService>();

        services.AddScoped<Services.V2.CategoryService>();

        services.AddScoped<IProductLinks, ProductLinks>();
        services.AddScoped<ICategoryLinks, CategoryLinks>();
        services.AddScoped<IReviewLinks, ReviewLinks>();
        services.AddScoped<IPriceHistoryLinks, PriceHistoryLinks>();

        services.AddScoped<ValidateMediaTypeAttribute>();
        services.AddScoped<ValidationFilterAttribute>();
    }

    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration) {
        services.AddMassTransit(busConfigurator => {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingAzureServiceBus((context, configurator) => {
                configurator.Host(configuration.GetConnectionString("AzureServiceBus"));
                configurator.ConfigureEndpoints(context);
            });
        });
    }

    public static void ConfigureCosmosDB(this IServiceCollection services, IConfiguration configuration) {
        var cosmosDbConfiguration = new CosmosDbConfiguration();
        configuration.Bind(CosmosDbConfiguration.Section, cosmosDbConfiguration);

        services.AddDbContext<ProductContext>(options =>
            options.UseCosmos(cosmosDbConfiguration.AccountEndpoint, cosmosDbConfiguration.AccountKey,
                cosmosDbConfiguration.DatabaseName)
        );
    }

    public static void ConfigureAzureStorage(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<AzureBlobStorageConfiguration>(configuration.GetSection("AzureBlobStorage"));
    }


    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration) {
        services.AddHealthChecks()
            .AddAzureCosmosDB(x => x.GetRequiredService<ProductContext>().Database.GetCosmosClient())
            //.AddAzureBlobStorage(blobStorageConfiguration.ConnectionString, blobStorageConfiguration.Container)
            .AddRedis(configuration.GetConnectionString("Redis"));
    }
    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options =>
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")));
    }

    public static void ConfigureSwagger(this IServiceCollection services) {
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Product API",
                Version = "v1",
                Description = "Product API by lewy256",
                Contact = new OpenApiContact {
                    Name = "lewy256",
                    Url = new Uri("https://github.com/lewy256"),
                }
            });

            s.SwaggerDoc("v2", new OpenApiInfo { Title = "Product API", Version = "v2" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);

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

    public static void AddCustomMediaTypes(this IServiceCollection services) {
        services.Configure<MvcOptions>(config => {

            var systemTextJsonOutputFormatter = config.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?
                .FirstOrDefault();

            if(systemTextJsonOutputFormatter != null) {
                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.lewy256.hateoas+json");
                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.lewy256.apiroot+json");

            }
        });
    }

    public static void ConfigureVersioning(this IServiceCollection services) {
        services.AddApiVersioning(opt => {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
        }).AddMvc(opt => {
            opt.Conventions.Controller<Controllers.V2.CategoryController>()
                .HasApiVersion(new ApiVersion(2, 0));
        });
    }

    public static void ConfigureOutputCaching(this IServiceCollection services, IConfiguration configuration) {
        services.AddOutputCache(opt => {
            opt.AddBasePolicy(bp => bp.Expire(TimeSpan.FromMinutes(5)).Tag("products"));
        }).AddStackExchangeRedisOutputCache(options => {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
    }


    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.Section, jwtConfiguration);

        var client = new SecretClient(new Uri(jwtConfiguration.KeyVaultUri),
            new DefaultAzureCredential());

        var secret = client.GetSecret(jwtConfiguration.SecretName);

        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value))
            };
        });
    }


    public static void ConfigureAuthorization(this IServiceCollection services) {
        services.AddAuthorization(options => {
            options.AddPolicy("RequireMultipleRoles", policy =>
                    policy.RequireRole(UserRole.Administrator, UserRole.Customer));
            options.AddPolicy("RequireAdministratorRole", policy =>
                    policy.RequireRole(UserRole.Administrator));
            options.AddPolicy("RequireCustomerRole", policy =>
                    policy.RequireRole(UserRole.Customer));
        });
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