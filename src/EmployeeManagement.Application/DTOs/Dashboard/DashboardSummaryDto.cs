namespace EmployeeManagement.Application.DTOs.Dashboard;

public sealed record DepartmentDistributionDto(
    int DepartmentId,
    string DepartmentName,
    int EmployeeCount,
    int ActiveEmployeeCount);

public sealed record DashboardSummaryDto(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    int TotalDepartments,
    IReadOnlyList<DepartmentDistributionDto> EmployeesByDepartment);