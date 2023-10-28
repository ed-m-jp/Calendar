namespace Calendar.Services
{
    //All possible result when calling a service method.
    public enum ServiceResultStatus
    {
        Ok,
        NotFound,
        Error,
        Unprocessable,
        BadRequest,
        Unauthorized,
    }
}
