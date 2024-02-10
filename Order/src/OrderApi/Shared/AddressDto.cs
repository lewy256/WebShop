namespace OrderApi.Shared;

public record AddressDto {
    public int AddressId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string PostalCode { get; init; }
    public string Phone { get; init; }
    public string Country { get; init; }
    public string City { get; init; }
}
