using FluentValidation.Results;

namespace ProductApi.Model.Responses;
public record class ValidationFailed(IEnumerable<ValidationFailure> Errors) {
}
