namespace OrderApi.Responses;

public class NotFoundResponse : ApiBaseResponse {
    public NotFoundResponse(int id, string entity)
       : base(StatusCodes.Status404NotFound,
           $"The {entity} with id: {id} doesn't exist in the database.") {
    }

    public NotFoundResponse(Guid id, string entity)
     : base(StatusCodes.Status404NotFound,
         $"The {entity} with id: {id} doesn't exist in the database.") {
    }
}
