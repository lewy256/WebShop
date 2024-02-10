namespace IdentityApi.Shared;

public record AuthenticationUserDto {
    public string UserName { get; init; }
    public string Password { get; init; }
}
