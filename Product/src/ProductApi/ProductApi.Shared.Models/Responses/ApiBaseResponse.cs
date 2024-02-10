using System.Text.Json.Serialization;

namespace ProductApi.Shared.Model.Responses;

public abstract class ApiBaseResponse {
    [JsonPropertyOrder(-1)]
    public int StatusCode { get; }
    [JsonPropertyOrder(-1)]
    public string Message { get; }
    public ApiBaseResponse(int statusCode, string message) {
        StatusCode = statusCode;
        Message = message;
    }
}
