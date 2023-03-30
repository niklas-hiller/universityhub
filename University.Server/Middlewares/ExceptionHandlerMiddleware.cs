using Newtonsoft.Json;
using System.Net;

namespace University.Server.Middlewares
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var response = context.Response;
                if (!response.HasStarted)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.ContentType = "application/json";
                    await response.WriteAsync(JsonConvert.SerializeObject(ex));
                }
                else
                {
                    _logger.LogWarning("Can't write error response. Response has already started.");
                }
            }
        }
    }
}
