namespace IdentityApi.Responses;

public class NotFoundResponse : ApiBaseResponse {
    public NotFoundResponse(Guid id, string entity)
       : base(StatusCodes.Status404NotFound,
           $"The {entity} with id: {id} doesn't exist in the database.") {
    }
}
