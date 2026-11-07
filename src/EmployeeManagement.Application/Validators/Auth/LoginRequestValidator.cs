using EmployeeManagement.Application.DTOs.Auth;

using FluentValidation;

namespace EmployeeManagement.Application.Validators.Auth;

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}