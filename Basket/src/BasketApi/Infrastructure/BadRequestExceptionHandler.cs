using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BasketApi.Infrastructure;

public sealed class BadRequestExceptionHandler : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) {
        if(exception is not BadHttpRequestException badRequestException) {
            return false;
        }

        Log.Error(
            badRequestException,
            "Exception occurred: {Message}",
            badRequestException.Message);

        var problemDetails = new ProblemDetails {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = badRequestException.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}