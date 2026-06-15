using CleanArchitecture.Contracts;
using Domain.comman;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult ProcessResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success());
            }

            return MapFailureResult(result.Error);
        }

        protected IActionResult ProcessResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<T>.Success(result.Data));
            }

            return MapFailureResult(result.Error);
        }

        private IActionResult MapFailureResult(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(error.Type),
                Detail = error.Description,
            };

            if (error is ValidationError validationError)
            {
                problemDetails.Extensions.Add("errors", validationError.Errors);
            }

            if (error.Metadata != null)
            {
                problemDetails.Extensions.Add("metadata", error.Metadata);
            }

            problemDetails.Extensions.Add("errorCode", error.Code);

            var traceId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            problemDetails.Extensions.Add("traceId", traceId);

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode,
                ContentTypes = { "application/problem+json" }
            };
        }

        private static string GetTitle(ErrorType errorType) => errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Unexpected => "Internal Server Error",
            _ => "Bad Request"
        };
    }
}
