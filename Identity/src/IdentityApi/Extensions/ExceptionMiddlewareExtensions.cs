using IdentityApi.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;

namespace IdentityApi.Extensions;

public static class ExceptionMiddlewareExtensions {
    public static void UseCustomExceptionHandler(this WebApplication app) {
        app.UseExceptionHandler(appError => {
            appError.Run(async context => {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if(contextFeature != null) {
                    Log.Error($"Something went wrong: {contextFeature.Error}");

                    if(contextFeature.Error is BadHttpRequestException) {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsJsonAsync(new BadRequestResponse("The JSON value is not in a supported format."));
                    }
                    else {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsJsonAsync(new InternalServerErrorResponse("Internal Server Error."));
                    }
                }
            });
        });
    }
}
