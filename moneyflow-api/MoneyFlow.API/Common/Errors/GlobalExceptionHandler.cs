using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Common;

namespace MoneyFlow.API.Common.Errors;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),

            ConflictException => (
                StatusCodes.Status409Conflict,
                "Resource conflict",
                exception.Message),

            ValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation error",
                exception.Message),

            BusinessRuleException => (
                StatusCodes.Status400BadRequest,
                "Business rule violation",
                exception.Message),

            DomainException => (
                StatusCodes.Status400BadRequest,
                "Domain rule violation",
                exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "An unexpected error occurred while processing the request.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}