using Asp.Versioning;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductApi.Configurations;
using ProductApi.Filters;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Service.Configurations;
using ProductApi.Service.DataShaping;
using ProductApi.Service.Interfaces;
using ProductApi.Service.V1;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Utility;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace ProductApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPriceHistoryService, PriceHistoryService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddScoped<Service.V2.ProductService>();
        services.AddScoped<Service.V2.CategoryService>();
        services.AddScoped<Service.V2.PriceHistoryService>();
        services.AddScoped<Service.V2.ReviewService>();

        services.AddScoped<Service.V3.CategoryService>();

        services.AddScoped<IFileService, FileService>();

        services.AddScoped<IProductLinks, ProductLinks>();
        services.AddScoped<ICategoryLinks, CategoryLinks>();
        services.AddScoped<IReviewLinks, ReviewLinks>();
        services.AddScoped<IPriceHistoryLinks, PriceHistoryLinks>();

        services.AddScoped<IDataShaper<ProductDto>, DataShaper<ProductDto>>();

        services.AddScoped<ValidateMediaTypeAttribute>();
        services.AddScoped<ValidationFilterAttribute>();
        services.AddScoped<MultipartFormDataAttribute>();


    }

    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration) {
        var azureServiceBusConfiguration = new AzureServiceBusConfiguration();
        configuration.Bind(AzureServiceBusConfiguration.Section, azureServiceBusConfiguration);

        services.AddMassTransit(busConfigurator => {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingAzureServiceBus((context, configurator) => {
                configurator.Host(azureServiceBusConfiguration.ConnectionString);
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

    public static void BindConfiguration(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<AzureBlobStorageConfiguration>(configuration.GetSection("AzureBlobStorage"));
    }

    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration) {
        var blobStorageConfiguration = new AzureBlobStorageConfiguration();
        configuration.Bind(AzureBlobStorageConfiguration.Section, blobStorageConfiguration);

        var redisConfiguration = new RedisConfiguration();
        configuration.Bind(RedisConfiguration.Section, redisConfiguration);

        services.AddHealthChecks()
            .AddAzureCosmosDB(x => x.GetRequiredService<ProductContext>().Database.GetCosmosClient())
            .AddAzureBlobStorage(blobStorageConfiguration.ConnectionString, blobStorageConfiguration.Container)
            .AddRedis(redisConfiguration.ConnectionString, redisConfiguration.InstanceName);
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
            s.SwaggerDoc("v3", new OpenApiInfo { Title = "Product API", Version = "v3" });

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
            opt.Conventions.Controller<Controllers.V2.ReviewController>()
                .HasApiVersion(new ApiVersion(2, 0));
            opt.Conventions.Controller<Controllers.V2.CategoryController>()
                .HasApiVersion(new ApiVersion(2, 0));
            opt.Conventions.Controller<Controllers.V3.CategoryController>()
                .HasApiVersion(new ApiVersion(3, 0));
            opt.Conventions.Controller<Controllers.V2.PriceHistoryController>()
                .HasApiVersion(new ApiVersion(2, 0));
            opt.Conventions.Controller<Controllers.V2.ProductController>()
                .HasApiVersion(new ApiVersion(2, 0));
        });
    }

    public static void ConfigureOutputCaching(this IServiceCollection services, IConfiguration configuration) {
        var redisConfiguration = new RedisConfiguration();

        configuration.Bind(RedisConfiguration.Section, redisConfiguration);

        services.AddOutputCache(opt => {
            opt.AddBasePolicy(bp => bp.Expire(TimeSpan.FromMinutes(5)).Tag("products"));
        }).AddStackExchangeRedisOutputCache(options => {
            options.Configuration = redisConfiguration.ConnectionString;
            options.InstanceName = redisConfiguration.InstanceName;
        });
    }



    public static void ConfigureRateLimitingOptions(this IServiceCollection services) {
        services.AddRateLimiter(opt => {
            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter",
                partition => new FixedWindowRateLimiterOptions {
                    AutoReplenishment = true,
                    PermitLimit = 30,
                    QueueLimit = 2,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    Window = TimeSpan.FromMinutes(1)
                }));

            opt.AddPolicy("SpecificPolicy", context =>
                RateLimitPartition.GetFixedWindowLimiter("SpecificLimiter",
                partition => new FixedWindowRateLimiterOptions {
                    AutoReplenishment = true,
                    PermitLimit = 3,
                    Window = TimeSpan.FromSeconds(10)
                }));

            opt.OnRejected = async (context, token) => {
                context.HttpContext.Response.StatusCode = 429;

                if(context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)) {
                    await context.HttpContext.Response
                        .WriteAsync($"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s).", token);
                }
                else {
                    await context.HttpContext.Response
                        .WriteAsync("Too many requests. Please try again later.", token);
                }
            };
        });
    }


    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        var jwtConfiguration = new JwtConfiguration();

        configuration.Bind(JwtConfiguration.Section, jwtConfiguration);

        var secret = "";

        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development")) {
            secret = configuration.GetValue<string>("SECRET");
        }
        else {
            var keyVaultConfiguration = new KeyVaultConfiguration();

            configuration.Bind(KeyVaultConfiguration.Section, keyVaultConfiguration);

            var client = new SecretClient(new Uri(keyVaultConfiguration.KeyVaultUri), new DefaultAzureCredential(includeInteractiveCredentials: true));
            secret = client.GetSecret(keyVaultConfiguration.SecretName).Value.Value;
        }

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
        });
    }
}