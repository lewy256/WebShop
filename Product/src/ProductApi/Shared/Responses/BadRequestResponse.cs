using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Shared.Responses;

public class BadRequestResponse : ProblemDetails {
    public BadRequestResponse(string message) {
        Status = StatusCodes.Status400BadRequest;
        Detail = message;
    }
}
