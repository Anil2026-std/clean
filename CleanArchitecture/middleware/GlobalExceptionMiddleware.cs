using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CleanArchitecture.middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IExceptionMapper exceptionMapper)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, exceptionMapper);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, IExceptionMapper exceptionMapper)
        {
            context.Response.ContentType = "application/problem+json";

            var mapped = exceptionMapper.Map(exception);
            context.Response.StatusCode = mapped.StatusCode;

            if (mapped.StatusCode >= 500)
            {
                _logger.LogError(exception, "An unexpected unhandled exception occurred.");
            }
            else
            {
                _logger.LogWarning(exception, "A domain or validation exception occurred: {ErrorCode}", mapped.ErrorCode);
            }

            var correlationId = context.Items["CorrelationId"]?.ToString()
                                ?? context.Request.Headers["X-Correlation-ID"].ToString()
                                ?? Activity.Current?.Id
                                ?? context.TraceIdentifier;

            var problemDetails = new ProblemDetails
            {
                Status = mapped.StatusCode,
                Title = mapped.Title,
                Detail = mapped.Detail
            };

            problemDetails.Extensions.Add("errorCode", mapped.ErrorCode);
            problemDetails.Extensions.Add("correlationId", correlationId);

            if (mapped.Errors != null)
            {
                problemDetails.Extensions.Add("errors", mapped.Errors);
            }

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
