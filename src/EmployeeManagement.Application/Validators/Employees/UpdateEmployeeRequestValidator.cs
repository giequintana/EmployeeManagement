using EmployeeManagement.Application.DTOs.Employees;
using FluentValidation;

namespace EmployeeManagement.Application.Validators.Employees;

internal sealed class UpdateEmployeeRequestValidator : EmployeeRequestValidatorBase<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(TimeProvider timeProvider) : base(timeProvider)
    {
        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("IsActive is required when updating an employee.");
    }
}