using Microsoft.AspNetCore.Mvc;

namespace ProductApi.Shared.Responses;

public class ValidationResponse : ProblemDetails {
    public ValidationResponse(IEnumerable<ValidationError> errors) {
        Detail = "Model is invalid";
        Status = StatusCodes.Status422UnprocessableEntity;
        Extensions = new Dictionary<string, object?> {
            { "errors",errors }
        };
    }
}

