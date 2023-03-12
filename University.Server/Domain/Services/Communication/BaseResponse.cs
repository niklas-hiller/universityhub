namespace University.Server.Domain.Services.Communication
{
    public abstract class BaseResponse
    {
        public int StatusCode { get; protected set; }
        public string Message { get; protected set; }

        public BaseResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
