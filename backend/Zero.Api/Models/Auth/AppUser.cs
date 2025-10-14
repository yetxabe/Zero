using Microsoft.AspNetCore.Identity;

namespace Zero.Api.Models.Auth;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IzaroCode { get; set; }
}