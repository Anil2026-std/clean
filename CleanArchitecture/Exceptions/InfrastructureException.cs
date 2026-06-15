using CleanArchitecture.Exceptions;
using Domain.exceptions;

namespace CleanArchitecture.Exceptions
{
    public class InfrastructureException : DomainException
    {
        public InfrastructureException(string message, Exception innerException)
            : base("infrastructure_error", message)
        {

        }
    }
}
