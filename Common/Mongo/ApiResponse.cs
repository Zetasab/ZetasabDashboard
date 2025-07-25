namespace ZetaDashboard.Common.Mongo
{
    public class ApiResponse<T>
    {
        public bool Result { get; set; }
        public T? Data { get; set; }
        public string Message { get;set; }

        public ApiResponse() { }

        public ApiResponse(T data, bool result = false, string? message = null)
        {
            Data = data;
            Result = result;
            Message = message;
        }
    }
}
