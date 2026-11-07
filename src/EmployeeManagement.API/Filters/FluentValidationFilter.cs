using System.Text.Json;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace EmployeeManagement.API.Filters;

internal sealed class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var services = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (services.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                context.ModelState.AddModelError(
                    JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName),
                    failure.ErrorMessage);
            }
        }

        if (!context.ModelState.IsValid)
        {
            var problemDetails = services
                .GetRequiredService<ProblemDetailsFactory>()
                .CreateValidationProblemDetails(
                    context.HttpContext,
                    context.ModelState,
                    StatusCodes.Status400BadRequest);

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest,
                ContentTypes = { "application/problem+json" }
            };

            return;
        }

        await next();
    }
}