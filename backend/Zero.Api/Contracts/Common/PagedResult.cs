namespace Zero.Api.Contracts.Common;

public record PagedResult<T>(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<T> Items
);