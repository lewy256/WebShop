using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Responses;

public class NotFoundResponse : ProblemDetails {
    public NotFoundResponse(Guid id, string entity) {
        Detail = $"The {entity} with id: {id} doesn't exist in the database.";
        Status = StatusCodes.Status404NotFound;
    }
}
