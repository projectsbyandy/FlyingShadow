using System.ComponentModel.DataAnnotations;

namespace FlyingShadow.Core.DTO.Authenticate;

public record RegisterRequest
{
    [EmailAddress]
    public required string Email { get; init; }
    
    [MinLength(5)]
    public required string Password { get; init; }
}