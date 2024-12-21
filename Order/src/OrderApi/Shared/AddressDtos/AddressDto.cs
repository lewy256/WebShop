namespace OrderApi.Shared.AddressDtos;

public record AddressDto {
    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string PostalCode { get; init; }
    public string PhoneNumber { get; init; }
    public string Country { get; init; }
    public string City { get; init; }
}
