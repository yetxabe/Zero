using System.ComponentModel.DataAnnotations;

namespace Zero.Api.Contracts.Admin;

public class UpdateUserDto
{
    [Required, EmailAddress] 
    public string Email { get; set; } = default!;
    [Required, MinLength(1)]
    public string FirstName { get; set; } = default!;
    [Required, MinLength(1)]
    public string LastName { get; set; } = default!;
    [Required, MinLength(1)] 
    public string IzaroCode { get; set; } = default!;
    public IList<string>? Roles { get; set; }
}