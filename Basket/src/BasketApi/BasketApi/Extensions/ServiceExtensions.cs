﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BasketApi.Configurations;
using BasketApi.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BasketApi.Extensions;

public static class ServiceExtensions {
    public static void ConfigureServices(this IServiceCollection services) {
        services.AddScoped<BasketService>();
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
                Title = "BasketAPI",
                Version = "v1",
                Description = "Basket API by lewy256",
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