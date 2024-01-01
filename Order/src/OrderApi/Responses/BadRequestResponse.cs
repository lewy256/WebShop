namespace OrderApi.Responses;

public class BadRequestResponse {
    public string Message { get; }

    public BadRequestResponse() {
        Message = $"Object is null.";
    }
}
