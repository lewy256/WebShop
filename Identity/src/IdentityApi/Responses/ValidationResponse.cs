using IdentityApi.Shared;

namespace IdentityApi.Responses;
public class ValidationResponse : ApiBaseResponse {
    public IEnumerable<ValidationError> Errors { get; }
    public ValidationResponse(IEnumerable<ValidationError> errors)
        : base(StatusCodes.Status422UnprocessableEntity, "Model is invalid.") {

        Errors = errors;
    }
}

