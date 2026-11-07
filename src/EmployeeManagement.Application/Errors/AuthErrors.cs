using EmployeeManagement.Application.Common;

namespace EmployeeManagement.Application.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password.");
}