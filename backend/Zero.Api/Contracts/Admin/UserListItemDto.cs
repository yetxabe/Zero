namespace Zero.Api.Contracts.Admin;

public record UserListItemDto
(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string IzaroCode,
    IList<string> Roles
);