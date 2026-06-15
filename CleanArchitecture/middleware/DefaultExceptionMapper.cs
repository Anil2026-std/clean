using CleanArchitecture.Exceptions;
using System.Net;
using FluentValidation;
using Domain.exceptions;
namespace CleanArchitecture.middleware
{
    public class DefaultExceptionMapper : IExceptionMapper
    {
        public (int StatusCode, string ErrorCode, string Title, string Detail, IDictionary<string, string[]>? Errors) Map(Exception exception)
        {
            switch (exception)
            {
                case ValidationException validationEx:
                    var errors = validationEx.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                    return (
                        StatusCode: (int)HttpStatusCode.BadRequest,
                        ErrorCode: "validation.error",
                        Title: "Validation Failed",
                        Detail: "One or more validation errors occurred.",
                        Errors: errors
                    );

                case DomainException domainEx:
                    var statusCode = domainEx switch
                    {
                        InfrastructureException => (int)HttpStatusCode.ServiceUnavailable,
                        _ => (int)HttpStatusCode.BadRequest
                    };

                    return (
                        StatusCode: statusCode,
                        ErrorCode: domainEx.ErrorCode,
                        Title: "Domain Rule Violation",
                        Detail: domainEx.Message,
                        Errors: null
                    );

                default:
                    return (
                        StatusCode: (int)HttpStatusCode.InternalServerError,
                        ErrorCode: "internal.server.error",
                        Title: "Internal Server Error",
                        Detail: "An unexpected error occurred.",
                        Errors: null
                    );
            }
        }
    }
}
