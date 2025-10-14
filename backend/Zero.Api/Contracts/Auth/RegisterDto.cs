namespace Zero.Api.Contracts.Auth;

public record RegisterDto(
    string Email, 
    string Password,
    string FirstName,
    string LastName,
    string IzaroCode
);