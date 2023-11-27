namespace OrderApi.Exceptions;


public sealed class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException(int customerId) :
        base($"The customer with id: {customerId} doesn't exist in the database.")
    {
    }
}