using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Errors;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Mappings;

namespace EmployeeManagement.Application.Services;

internal sealed class EmployeeService(
    IEmployeeRepository employees,
    IDepartmentRepository departments,
    IUnitOfWork unitOfWork) : IEmployeeService
{
    public Task<PagedResult<EmployeeSummaryDto>> GetPagedAsync(
        EmployeeQueryParameters parameters,
        CancellationToken cancellationToken = default) =>
        employees.GetPagedAsync(parameters, cancellationToken);

    public async Task<Result<EmployeeDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetDetailsByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(id);
        }

        return employee;
    }

    public async Task<Result<EmployeeDetailsDto>> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = request.ToEntity();

        if (!await departments.ExistsAsync(employee.DepartmentId, cancellationToken))
        {
            return EmployeeErrors.DepartmentNotFound(employee.DepartmentId);
        }

        if (await employees.EmployeeNumberExistsAsync(employee.EmployeeNumber, cancellationToken))
        {
            return EmployeeErrors.DuplicateEmployeeNumber(employee.EmployeeNumber);
        }

        if (await employees.EmailExistsAsync(employee.Email, cancellationToken: cancellationToken))
        {
            return EmployeeErrors.DuplicateEmail(employee.Email);
        }

        employees.Add(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadDetailsAsync(employee.Id, cancellationToken);
    }

    public async Task<Result<EmployeeDetailsDto>> UpdateAsync(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(id);
        }

        if (employee.DepartmentId != request.DepartmentId
            && !await departments.ExistsAsync(request.DepartmentId, cancellationToken))
        {
            return EmployeeErrors.DepartmentNotFound(request.DepartmentId);
        }

        var email = EmployeeMappings.NormalizeEmail(request.Email);

        if (await employees.EmailExistsAsync(email, id, cancellationToken))
        {
            return EmployeeErrors.DuplicateEmail(email);
        }

        request.ApplyTo(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadDetailsAsync(id, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employees.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(id);
        }

        employees.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<EmployeeDetailsDto> LoadDetailsAsync(int id, CancellationToken cancellationToken) =>
        await employees.GetDetailsByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Employee {id} was saved but could not be reloaded.");
}