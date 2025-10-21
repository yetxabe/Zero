using System.ComponentModel.DataAnnotations;

namespace Zero.Api.Contracts.Admin;

public class CreateUserDto
{
    [Required, EmailAddress] 
    public string Email { get; set; } = default!;
    [Required, MinLength(1)]
    public string FirstName { get; set; } = default!;
    [Required, MinLength(1)]
    public string LastName { get; set; } = default!;
    [Required, MinLength(1)] 
    public string IzaroCode { get; set; } = default!;
    [Required, MinLength(6)]
    public string Password { get; set; } = default!;
}