using Microsoft.AspNetCore.Diagnostics;
using OrderApi.Exceptions;
using OrderApi.Models;
using Serilog;
using System.Net;

namespace OrderApi.Extensions;

public static class ExceptionMiddlewareExtension {
    public static void ConfigureExceptionHandler(this WebApplication app) {
        app.UseExceptionHandler(appError => {
            appError.Run(async context => {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if(contextFeature != null) {
                    context.Response.StatusCode = contextFeature.Error switch {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    };
                    Log.Error($"Something went wrong: {contextFeature.Error}");
                    await context.Response.WriteAsync(new ErrorDetail() {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message
                    }.ToString());
                }
            });
        });
    }
}