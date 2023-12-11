namespace OrderApi.Exceptions;

public sealed class OrderNotFoundException : NotFoundException {
    public OrderNotFoundException(Guid id) :
        base($"The order with id: {id} doesn't exist in the database.") {
    }
}