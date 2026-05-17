using System.Net;
using System.Text.Json;
using InteractHub.Application.Common;
using InteractHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace InteractHub.Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(
                    "Validation failed",
                    new[] { validationEx.Message },
                    context.TraceIdentifier)),

            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(
                    "Bad request",
                    new[] { badRequestEx.Message },
                    context.TraceIdentifier)),

            UnauthorizedAccessException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                ApiResponse.Fail(
                    "Unauthorized",
                    new[] { unauthorizedEx.Message },
                    context.TraceIdentifier)),

            UnauthorizedException unauthorizedEx2 => (
                HttpStatusCode.Unauthorized,
                ApiResponse.Fail(
                    "Unauthorized",
                    new[] { unauthorizedEx2.Message },
                    context.TraceIdentifier)),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                ApiResponse.Fail(
                    "Not found",
                    new[] { notFoundEx.Message },
                    context.TraceIdentifier)),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                ApiResponse.Fail(
                    "Conflict",
                    new[] { conflictEx.Message },
                    context.TraceIdentifier)),

            DbUpdateException dbUpdateEx => (
                HttpStatusCode.Conflict,
                ApiResponse.Fail(
                    "Database update conflict",
                    new[] { _environment.IsDevelopment() ? dbUpdateEx.Message : "A database conflict occurred." },
                    context.TraceIdentifier)),

            KeyNotFoundException keyNotFoundEx => (
                HttpStatusCode.NotFound,
                ApiResponse.Fail(
                    "Not found",
                    new[] { keyNotFoundEx.Message },
                    context.TraceIdentifier)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse.Fail(
                    "Server error",
                    new[] { _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred." },
                    context.TraceIdentifier))
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}