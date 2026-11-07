using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Errors;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Mappings;

namespace EmployeeManagement.Application.Services;

internal sealed class DepartmentService(
    IDepartmentRepository departments,
    IUnitOfWork unitOfWork) : IDepartmentService
{
    public Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        departments.GetAllAsync(cancellationToken);

    public async Task<Result<DepartmentDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await departments.GetDtoByIdAsync(id, cancellationToken);

        if (department is null)
        {
            return DepartmentErrors.NotFound(id);
        }

        return department;
    }

    public async Task<Result<DepartmentDto>> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = request.ToEntity();

        if (await departments.NameExistsAsync(department.DepartmentName, cancellationToken: cancellationToken))
        {
            return DepartmentErrors.DuplicateName(department.DepartmentName);
        }

        departments.Add(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadDtoAsync(department.Id, cancellationToken);
    }

    public async Task<Result<DepartmentDto>> UpdateAsync(
        int id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);

        if (department is null)
        {
            return DepartmentErrors.NotFound(id);
        }

        var name = DepartmentMappings.NormalizeName(request.DepartmentName);

        if (await departments.NameExistsAsync(name, id, cancellationToken))
        {
            return DepartmentErrors.DuplicateName(name);
        }

        request.ApplyTo(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LoadDtoAsync(id, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);

        if (department is null)
        {
            return DepartmentErrors.NotFound(id);
        }

        if (await departments.HasEmployeesAsync(id, cancellationToken))
        {
            return DepartmentErrors.HasEmployees(id);
        }

        departments.Remove(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<DepartmentDto> LoadDtoAsync(int id, CancellationToken cancellationToken) =>
        await departments.GetDtoByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Department {id} was saved but could not be reloaded.");
}