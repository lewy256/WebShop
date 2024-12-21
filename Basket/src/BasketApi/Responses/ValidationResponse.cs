using BasketApi.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BasketApi.Responses;
public class ValidationResponse : ProblemDetails {
    public ValidationResponse(IEnumerable<ValidationError> errors) {
        Detail = "Model is invalid";
        Status = StatusCodes.Status422UnprocessableEntity;
        Extensions = new Dictionary<string, object?> {
            { "errors",errors }
        };
    }
}