using EmployeeManagement.API.Authorization;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[Route("api/departments")]
public sealed class DepartmentsController(IDepartmentService departmentService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<DepartmentDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var departments = await departmentService.GetAllAsync(cancellationToken);
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<DepartmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await departmentService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error);
    }

    [HttpPost]
    [Authorize(Policy = Policies.ManageDepartments)]
    [ProducesResponseType<DepartmentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await departmentService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : HandleFailure(result.Error);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.ManageDepartments)]
    [ProducesResponseType<DepartmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await departmentService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.ManageDepartments)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await departmentService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result.Error);
    }
}