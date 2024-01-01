namespace OrderApi.Responses;
public record class ValidationFailed(IEnumerable<ValidationError> Errors) {
}
