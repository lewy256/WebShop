namespace OrderApi.Responses;
public class NotFoundResponse {
    public string Message { get; }
    public NotFoundResponse(Guid id, string entity) {
        Message = $"The {entity} with id: {id} doesn't exist in the database.";
    }

}
