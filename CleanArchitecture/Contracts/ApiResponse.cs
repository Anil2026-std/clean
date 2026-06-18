namespace CleanArchitecture.Contracts
{
    public class ApiResponse
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public string? ErrorCode { get; init; }
        public object? Errors { get; init; }

        public static ApiResponse Success(string? message = null)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message ?? "Operation successful."
            };
        }

    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; init; }

        public static ApiResponse<T> Success(T? data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message ?? "Operation successful."
            };
        }

    }
}
