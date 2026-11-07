using EmployeeManagement.Application.DTOs.Employees;
using FluentValidation;

namespace EmployeeManagement.Application.Validators.Employees;

internal abstract class EmployeeRequestValidatorBase<T> : AbstractValidator<T>
    where T : IEmployeeRequest
{
    private const int MinimumAgeAtHire = 18;
    private const int MaximumAge = 100;
    private const decimal MaximumSalary = 10_000_000m;

    protected EmployeeRequestValidatorBase(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .Matches(@"^[0-9+\-\s()]+$")
            .WithMessage("Phone number may contain only digits, spaces, and + - ( ).")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(today)
            .WithMessage("Date of birth must be in the past.")
            .GreaterThan(today.AddYears(-MaximumAge))
            .WithMessage($"Date of birth cannot be more than {MaximumAge} years ago.");

        RuleFor(x => x.HireDate)
            .NotEmpty()
            .LessThanOrEqualTo(today.AddYears(1))
            .WithMessage("Hire date cannot be more than one year in the future.")
            .Must((request, hireDate) => hireDate >= request.DateOfBirth.AddYears(MinimumAgeAtHire))
            .WithMessage($"Employee must be at least {MinimumAgeAtHire} years old on the hire date.")
            .When(x => x.DateOfBirth != default, ApplyConditionTo.CurrentValidator);

        RuleFor(x => x.Position)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(MaximumSalary)
            .PrecisionScale(18, 2, ignoreTrailingZeros: true);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("A department must be selected.");
    }
}