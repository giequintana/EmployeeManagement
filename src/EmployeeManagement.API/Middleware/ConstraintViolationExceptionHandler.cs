using EmployeeManagement.Application.Common.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Middleware;

internal sealed class ConstraintViolationExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ConstraintViolationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ConstraintViolationException violation)
        {
            return false;
        }

        logger.LogWarning(
            exception,
            "Database {Kind} constraint violation on {Method} {Path}",
            violation.Kind,
            httpContext.Request.Method,
            httpContext.Request.Path);

        var (statusCode, errorCode, detail) = violation.Kind switch
        {
            ConstraintViolationKind.Unique => (
                StatusCodes.Status409Conflict,
                "Database.UniqueViolation",
                "A record with the same unique value already exists."),
            ConstraintViolationKind.ForeignKey => (
                StatusCodes.Status409Conflict,
                "Database.ForeignKeyViolation",
                "The operation conflicts with related data."),
            _ => (
                StatusCodes.Status400BadRequest,
                "Database.CheckViolation",
                "One or more values violate a data rule.")
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Detail = detail,
                Extensions = { ["errorCode"] = errorCode }
            }
        });
    }
}