using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Contracts.Roles;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderApi.Infrastructure;
using OrderApi.Infrastructure.Configurations.AppSettings;
using System.Diagnostics;
using System.Text;

namespace OrderApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services) {
        services.AddSwaggerGen(s => {
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Order API",
                Version = "v1",
                Description = "Order API by lewy256",
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
        services.AddDbContext<OrderContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AzureSQL")));
    }

    public static void ConfigureHealthCheck(this IServiceCollection services, IConfiguration configuration) {
        services.AddHealthChecks().AddSqlServer(configuration.GetConnectionString("AzureSQL"));
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.Section, jwtConfiguration);

        var client = new SecretClient(new Uri(jwtConfiguration.KeyVaultUri),
            new DefaultAzureCredential());

        var secret = client.GetSecret(jwtConfiguration.SecretName);

        services
            .AddAuthentication(opt => {
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

    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration) {
        services.AddMassTransit(x => {
            //x.AddEntityFrameworkOutbox<OrderContext>(o => {
            //    o.QueryDelay = TimeSpan.FromSeconds(1);
            //    o.UsePostgres().UseBusOutbox();
            //});

            x.UsingAzureServiceBus((context, configurator) => {
                configurator.Host(configuration.GetConnectionString("AzureServiceBus"));
                configurator.ConfigureEndpoints(context);
            });
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
}