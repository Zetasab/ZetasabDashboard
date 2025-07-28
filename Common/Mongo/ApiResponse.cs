namespace ZetaDashboard.Common.Mongo
{
    public enum ResponseStatus
    {
        Unknown = 0,
        Ok = 200,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalError = 500
    }
    public class ApiResponse<T>
    {
        public ResponseStatus Result { get; set; }
        public T? Data { get; set; }
        public string Message { get;set; }

        public ApiResponse() { }

        public ApiResponse(T data, ResponseStatus result = ResponseStatus.BadRequest, string? message = null)
        {
            Data = data;
            Result = result;
            Message = message;
        }
    }
}
