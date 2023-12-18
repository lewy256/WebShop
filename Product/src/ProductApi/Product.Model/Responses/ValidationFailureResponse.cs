namespace ProductApi.Model.Responses;
public class ValidationFailureResponse {
    public IEnumerable<ValidationResponse> Errors { get; set; }
}
