using FluentValidation.Results;
using ProductApi.Model.Responses;

namespace ProductApi.Extensions;

public static class ResponseMappingExtension {
    public static ErrorResponse MapToResponse(this NotFoundResponse notFoundResponse) {
        return new ErrorResponse {
            Message = notFoundResponse.Message,
            StatusCode = StatusCodes.Status404NotFound
        };
    }

    public static ValidationFailureResponse MapToResponse(this IEnumerable<ValidationFailure> validationFailures) {
        return new ValidationFailureResponse {
            Errors = validationFailures.Select(f => new ValidationResponse {
                PropertyName = f.PropertyName,
                Message = f.ErrorMessage
            })
        };
    }

}
