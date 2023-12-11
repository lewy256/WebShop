using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductApi.Interfaces;
using ProductApi.Model;
using ProductApi.Service;
using ProductApi.Utility;
using System.Reflection;

namespace ProductApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductLinks, ProductLinks>();

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
            }
        });
    }


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