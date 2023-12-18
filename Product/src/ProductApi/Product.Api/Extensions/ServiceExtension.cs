using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderApi.ActionFilters;
using ProductApi.ActionFilters;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Model.Entities;
using ProductApi.Model.Validators;
using ProductApi.Service;
using ProductApi.Service.DataShaping;
using ProductApi.Shared.Model.ProductDtos;
using ProductApi.Shared.Model.ReviewDtos;
using ProductApi.Utility;
using System.Reflection;

namespace ProductApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPriceHistoryService, PriceHistoryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IProductLinks, ProductLinks>();
        services.AddScoped<IDataShaper<ProductDto>, DataShaper<ProductDto>>();
        services.AddScoped<IDataShaper<ReviewDto>, DataShaper<ReviewDto>>();
        services.AddScoped<ValidateMediaTypeAttribute>();
        services.AddScoped<ValidationFilterAttribute>();
        services.AddScoped<IValidator<Category>, CreateCategoryValidator>();
        services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();
        services.AddScoped<IValidator<UpdateProductDto>, UpdateProductValidator>();
        services.AddScoped<IValidator<ProductParameters>, ProductParametersValidator>();

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

    public static void ConfigureSwagger(this IServiceCollection services) {
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Product API",
                Version = "v1",
                Description = "Product API by lewy256",
                Contact = new OpenApiContact {
                    Name = "lewy256",
                    Url = new Uri("https://github.com/lewy256/WebShop"),
                }
            });

            s.SwaggerDoc("v2", new OpenApiInfo { Title = "Test API", Version = "v2" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);
            /*
                        s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                            In = ParameterLocation.Header,
                            Description = "Place to add JWT with Bearer",
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "Bearer"
                        });

                        s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                    Name = "Bearer",
                                },
                                new List<string>()
                            }
                        });*/
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
        }).AddMvc();
    }

    public static void ConfigureOutputCaching(this IServiceCollection services, IConfigurationSection configurationSection) {
        services.AddOutputCache(opt => {
            opt.AddBasePolicy(bp => bp.Expire(TimeSpan.FromMinutes(5)).Tag("products"));
        }).AddStackExchangeRedisOutputCache(options => {
            options.Configuration = configurationSection["RedisCache"];
            options.InstanceName = configurationSection["InstanceName"];
        });
    }



    //public static void ConfigureRateLimitingOptions(this IServiceCollection services) {
    //    services.AddRateLimiter(opt => {
    //        opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    //            RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter",
    //            partition => new FixedWindowRateLimiterOptions {
    //                AutoReplenishment = true,
    //                PermitLimit = 30,
    //                QueueLimit = 2,
    //                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    //                Window = TimeSpan.FromMinutes(1)
    //            }));

    //        opt.AddPolicy("SpecificPolicy", context =>
    //            RateLimitPartition.GetFixedWindowLimiter("SpecificLimiter",
    //            partition => new FixedWindowRateLimiterOptions {
    //                AutoReplenishment = true,
    //                PermitLimit = 3,
    //                Window = TimeSpan.FromSeconds(10)
    //            }));

    //        opt.OnRejected = async (context, token) => {
    //            context.HttpContext.Response.StatusCode = 429;

    //            if(context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
    //                await context.HttpContext.Response
    //                    .WriteAsync($"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s).", token);
    //            else
    //                await context.HttpContext.Response
    //                    .WriteAsync("Too many requests. Please try again later.", token);
    //        };
    //    });
    //}


    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        /*        var jwtSettings = configuration.GetSection("JwtSettings");

                var secretName = jwtSettings["secretName"];
                var keyVaultUri = jwtSettings["keyVaultUri"];

                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(includeInteractiveCredentials: true));
                var secret = client.GetSecret(secretName);

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

                        ValidIssuer = jwtSettings["validIssuer"],
                        ValidAudience = jwtSettings["validAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret.Value.Value))
                    };
                });*/
    }
}