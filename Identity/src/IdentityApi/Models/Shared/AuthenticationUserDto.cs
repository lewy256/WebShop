using System.ComponentModel.DataAnnotations;

namespace IdentityApi.Models.Shared;

public record AuthenticationUserDto {
    [Required(ErrorMessage = "User name is required")]
    public string? UserName { get; init; }
    [Required(ErrorMessage = "Password name is required")]
    public string? Password { get; init; }
}
