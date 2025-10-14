namespace Zero.Api.Contracts.Auth;

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAtUtc
);