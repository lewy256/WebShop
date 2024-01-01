namespace ProductApi.Model.Responses;
public record class ValidationFailed(IEnumerable<ValidationError> Errors) {
}

