using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Responses;

public class BadRequestResponse : ProblemDetails {
    public BadRequestResponse() {
        Detail = "patchDoc object sent from client is null.";
        Status = StatusCodes.Status400BadRequest;
    }
}
