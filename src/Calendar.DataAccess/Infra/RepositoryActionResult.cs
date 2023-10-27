namespace Calendar.DataAccess.Infra
{
    public class RepositoryActionResult
    {
        public bool IsOk 
            => Status == RepositoryActionStatus.Ok;

        public bool IsNotFound
            => Status == RepositoryActionStatus.NotFound;

        public bool IsError
            => Status == RepositoryActionStatus.Error;

        public Exception? Exception { get; }

        public string ErrorMessage { get; }

        public RepositoryActionStatus Status { get; }

        private RepositoryActionResult(Exception exception, string errorMessage, RepositoryActionStatus status)
        {
            Exception = exception;
            ErrorMessage = errorMessage;
            Status = status;
        }

        private RepositoryActionResult(string errorMessage, RepositoryActionStatus status)
        {
            ErrorMessage = errorMessage;
            Status = status;
        }

        internal static RepositoryActionResult NotFound(int id)
            => new(id.ToString(), RepositoryActionStatus.NotFound);

        internal static RepositoryActionResult NotFound(string id)
            => new(id, RepositoryActionStatus.NotFound);

        internal static RepositoryActionResult Ok()
            => new(string.Empty, RepositoryActionStatus.Ok);

        internal static RepositoryActionResult Error(Exception exception, string errorMessage)
            => new(exception, errorMessage, RepositoryActionStatus.Error);

        internal static RepositoryActionResult Error(string errorMessage)
            => new(errorMessage, RepositoryActionStatus.Error);
    }


    public class RepositoryActionResult<T> where T : class
    {
        public bool IsOk
            => Status == RepositoryActionStatus.Ok;

        public bool IsNotFound
            => Status == RepositoryActionStatus.NotFound;

        public bool IsError
            => Status == RepositoryActionStatus.Error;

        public T? Entity { get; }

        public Exception? Exception { get; }

        public string? ErrorMessage { get; }

        public RepositoryActionStatus Status { get; }

        private RepositoryActionResult(Exception exception, string errorMessage, RepositoryActionStatus status)
        {
            Entity = null;
            Exception = exception;
            Status = status;
            ErrorMessage = errorMessage;
        }

        private RepositoryActionResult(T entity, RepositoryActionStatus status)
        {
            Entity = entity;
            Status = status;
        }

        private RepositoryActionResult(string errorMessage, RepositoryActionStatus status)
        {
            ErrorMessage = errorMessage;
            Status = status;
        }

        private RepositoryActionResult(RepositoryActionStatus status)
        {
            Status = status;
        }

        public static RepositoryActionResult<T> NotFound(int id)
            => new(id.ToString(), RepositoryActionStatus.NotFound);

        public static RepositoryActionResult<T> NotFound(string errorMessage)
            => new(errorMessage, RepositoryActionStatus.NotFound);

        public static RepositoryActionResult<T> Ok(T responseData)
            => new(responseData, RepositoryActionStatus.Ok);

        internal static RepositoryActionResult<T> Ok()
            => new(RepositoryActionStatus.Ok);

        public static RepositoryActionResult<T> Error(Exception exception, string errorMessage)
            => new(exception, errorMessage, RepositoryActionStatus.Error);

        internal static RepositoryActionResult<T> Error(string errorMessage)
            => new(errorMessage, RepositoryActionStatus.Error);

    }
}
