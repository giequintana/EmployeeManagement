using EmployeeManagement.Application.DTOs.Employees;
using FluentValidation;

namespace EmployeeManagement.Application.Validators.Employees;

internal sealed class CreateEmployeeRequestValidator : EmployeeRequestValidatorBase<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(TimeProvider timeProvider) : base(timeProvider)
    {
        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^\s*[A-Za-z0-9-]+\s*$")
            .WithMessage("Employee number may contain only letters, digits, and hyphens.");
    }
}