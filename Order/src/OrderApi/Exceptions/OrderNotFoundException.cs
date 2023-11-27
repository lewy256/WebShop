namespace OrderApi.Exceptions;

public sealed class OrderNotFoundException : NotFoundException {
    public OrderNotFoundException(int offerId) :
        base($"The offer with id: {offerId} doesn't exist in the database.") {
    }
}