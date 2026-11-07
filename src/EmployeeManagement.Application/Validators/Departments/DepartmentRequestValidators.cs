using EmployeeManagement.Application.DTOs.Departments;
using FluentValidation;

namespace EmployeeManagement.Application.Validators.Departments;

internal abstract class DepartmentRequestValidatorBase<T> : AbstractValidator<T>
    where T : IDepartmentRequest
{
    protected DepartmentRequestValidatorBase()
    {
        RuleFor(x => x.DepartmentName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}

internal sealed class CreateDepartmentRequestValidator : DepartmentRequestValidatorBase<CreateDepartmentRequest>;

internal sealed class UpdateDepartmentRequestValidator : DepartmentRequestValidatorBase<UpdateDepartmentRequest>;