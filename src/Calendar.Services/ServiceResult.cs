using Calendar.DataAccess.Infra;

namespace Calendar.Services
{
    public class ServiceResult<T> where T : class
    {
        public bool IsOk => Status == ServiceResultStatus.Ok;

        public bool IsNotFound => Status == ServiceResultStatus.NotFound;

        public bool IsError => Status == ServiceResultStatus.Error;

        public bool IsUnprocessable => Status == ServiceResultStatus.Unprocessable;

        public bool IsBadRequest => Status == ServiceResultStatus.BadRequest;

        public bool IsUnauthorized => Status == ServiceResultStatus.Unauthorized;

        public T? Data { get; }

        public Exception? Exception { get; }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => string.IsNullOrEmpty(_errorMessage) && Exception != null ? Exception.Message : _errorMessage;
            set => _errorMessage = value;
        }

        public ServiceResultStatus Status { get; }

        private ServiceResult(Exception exception, ServiceResultStatus status)
        {
            Data = default;
            Exception = exception;
            Status = status;
        }

        private ServiceResult(T data, ServiceResultStatus status)
        {
            Data = data;
            Status = status;
        }

        private ServiceResult(string? errorMessage, ServiceResultStatus status)
        {
            ErrorMessage = errorMessage;
            Status = status;
        }

        private ServiceResult(ServiceResultStatus status)
        {
            Status = status;
        }

        internal static ServiceResult<T> NotFound()
            => new(ServiceResultStatus.NotFound);

        public static ServiceResult<T> NotFound(string errorMessage)
            => new(errorMessage, ServiceResultStatus.NotFound);

        internal static ServiceResult<T> Error(Exception exception)
            => new(exception, ServiceResultStatus.Error);

        internal static ServiceResult<T> Ok(T data)
            => new(data, ServiceResultStatus.Ok);

        internal static ServiceResult<T> Error(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.Error);

        internal static ServiceResult<T> Unprocessable(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.Unprocessable);

        internal static ServiceResult<T> BadRequest(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.BadRequest);

        internal static ServiceResult<T> Unauthorized(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.Unauthorized);

        internal static ServiceResult<T> FromRepositoryActionResult<TU>(RepositoryActionResult<TU> repositoryActionResult, Func<T> data, string? message = null) where TU : class
        {
            return repositoryActionResult.Status switch
            {
                RepositoryActionStatus.Ok => Ok(data()),
                RepositoryActionStatus.NotFound => NotFound(),
                RepositoryActionStatus.Error => message != null ? Error(message) : repositoryActionResult.Exception != null
                                        ? Error(repositoryActionResult.Exception)
                                        : Error(repositoryActionResult.ErrorMessage),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }


    public class ServiceResult
    {
        public bool IsOk => Status == ServiceResultStatus.Ok;

        public bool IsNotFound => Status == ServiceResultStatus.NotFound;

        public bool IsError => Status == ServiceResultStatus.Error;

        public bool IsUnprocessable => Status == ServiceResultStatus.Unprocessable;

        public bool IsBadRequest => Status == ServiceResultStatus.Unprocessable;

        public bool IsUnauthorized => Status == ServiceResultStatus.Unauthorized;

        public Exception? Exception { get; }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => string.IsNullOrEmpty(_errorMessage) && Exception != null ? Exception.Message : _errorMessage;
            set => _errorMessage = value;
        }

        public ServiceResultStatus Status { get; }

        private ServiceResult(Exception exception, ServiceResultStatus status)
        {
            Exception = exception;
            Status = status;
        }

        private ServiceResult(ServiceResultStatus status)
        {
            Status = status;
        }

        private ServiceResult(string? errorMessage, ServiceResultStatus status)
        {
            ErrorMessage = errorMessage;
            Status = status;
        }

        internal static ServiceResult NotFound()
            => new(ServiceResultStatus.NotFound);

        internal static ServiceResult Error(Exception exception)
            => new(exception, ServiceResultStatus.Error);

        internal static ServiceResult Ok()
            => new(ServiceResultStatus.Ok);

        internal static ServiceResult Error(string errorMessage)
            => new(errorMessage, ServiceResultStatus.Error);

        internal static ServiceResult Unprocessable(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.Unprocessable);

        internal static ServiceResult BadRequest(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.BadRequest);

        internal static ServiceResult Unauthorized(string? errorMessage = null)
            => new(errorMessage, ServiceResultStatus.Unauthorized);

        internal static ServiceResult FromRepositoryActionResult(RepositoryActionResult repositoryActionResult, string? message = null)
        {
            return repositoryActionResult.Status switch
            {
                RepositoryActionStatus.Ok => Ok(),
                RepositoryActionStatus.NotFound => NotFound(),
                RepositoryActionStatus.Error => message != null ? Error(message) : repositoryActionResult.Exception != null
                                        ? Error(repositoryActionResult.Exception)
                                        : Error(repositoryActionResult.ErrorMessage),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
