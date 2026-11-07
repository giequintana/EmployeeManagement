namespace EmployeeManagement.Application.DTOs.Employees;

public sealed record EmployeeQueryParameters
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    private readonly int _pageNumber = 1;
    private readonly int _pageSize = DefaultPageSize;

    public string? Search { get; init; }

    public int? DepartmentId { get; init; }

    public bool? IsActive { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
    }
}