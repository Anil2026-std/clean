namespace CleanArchitecture.middleware
{

    public interface IExceptionMapper
    {
        (int StatusCode, string ErrorCode, string Title, string Detail, IDictionary<string, string[]>? Errors) Map(Exception exception);
    }
}
