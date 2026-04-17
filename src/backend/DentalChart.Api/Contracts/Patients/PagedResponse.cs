namespace DentalChart.Api.Contracts.Patients;

public sealed record PagedResponse<T>(IReadOnlyList<T> Data, PaginationMeta Pagination);
public sealed record PaginationMeta(int Page, int PageSize, int TotalCount, int TotalPages);
