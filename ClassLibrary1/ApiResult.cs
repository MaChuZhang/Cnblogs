namespace Cnblogs.ApiModel
{
    public class ApiResult<T> : ApiResult
    {
        public new static ApiResult<T> Error(string message)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = message,
            };
        }

        public T Data { get; set; }
    }

    public class ApiResult
    {
        public static ApiResult Error(string message)
        {
            return new ApiResult
            {
                Success = false,
                Message = message,
            };
        }

        public static ApiResult<T> Ok<T>(T data)
        {
            return new ApiResult<T>()
            {
                Success = true,
                Message = "",
                Data = data
            };
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        //public List<string> ErrorBarCodes { get; set; }
    }
}
