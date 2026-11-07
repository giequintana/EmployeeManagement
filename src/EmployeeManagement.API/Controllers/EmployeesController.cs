using EmployeeManagement.API.Authorization;
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[Route("api/employees")]
public sealed class EmployeesController(IEmployeeService employeeService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<EmployeeSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] EmployeeQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetPagedAsync(parameters, cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<EmployeeDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error);
    }

    [HttpPost]
    [Authorize(Policy = Policies.ManageEmployees)]
    [ProducesResponseType<EmployeeDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await employeeService.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : HandleFailure(result.Error);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.ManageEmployees)]
    [ProducesResponseType<EmployeeDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.ManageEmployees)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await employeeService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result.Error);
    }
}