﻿using Microsoft.AspNetCore.Diagnostics;
using ProductApi.Shared.Model.Responses;
using Serilog;
using System.Net;

namespace ProductApi.Extensions;

public static class ExceptionMiddlewareExtensions {
    public static void UseCustomExceptionHandler(this WebApplication app) {
        app.UseExceptionHandler(appError => {
            appError.Run(async context => {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if(contextFeature is not null) {
                    Log.Error($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsJsonAsync(
                        new InternalServerErrorResponse("Internal Server Error.")
                    );
                }
            });
        });
    }
}
