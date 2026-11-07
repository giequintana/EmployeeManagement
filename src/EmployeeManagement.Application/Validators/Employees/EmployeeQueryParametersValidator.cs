using EmployeeManagement.Application.DTOs.Employees;
using FluentValidation;

namespace EmployeeManagement.Application.Validators.Employees;

internal sealed class EmployeeQueryParametersValidator : AbstractValidator<EmployeeQueryParameters>
{
    public EmployeeQueryParametersValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .When(x => x.DepartmentId.HasValue);
    }
}