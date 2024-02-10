using Microsoft.AspNetCore.Http;

namespace ProductApi.Shared.Model.Responses;

public class ValidationResponse : ApiBaseResponse {
    public IEnumerable<ValidationError> Errors { get; }
    public ValidationResponse(IEnumerable<ValidationError> errors)
        : base(StatusCodes.Status422UnprocessableEntity, "Model is invalid.") {

        Errors = errors;
    }
}

