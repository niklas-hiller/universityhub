using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using University.Server.Exceptions;

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

                var problem = new ProblemDetails()
                {
                    Detail = ex.Message
                };

                switch (ex)
                {
                    case BadRequestException e:
                        problem.Type = "Bad Request";
                        problem.Title = "Some conditions were not fulfilled.";
                        problem.Status = (int)e.StatusCode;
                        break;
                    case NotFoundException e:
                        problem.Type = "Not Found";
                        problem.Title = "Entity not found.";
                        problem.Status = (int)e.StatusCode;
                        break;
                    case InternalServerException e:
                        problem.Type = "Internal Server Error";
                        problem.Title = "Server ran into unexpected error.";
                        problem.Status = (int)e.StatusCode;
                        break;
                    default:
                        problem.Type = "Internal Server Error";
                        problem.Title = "Server ran into really unexpected error.";
                        problem.Status = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var response = context.Response;
                if (!response.HasStarted)
                {
                    response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;
                    response.ContentType = "application/json";
                    await response.WriteAsync(JsonConvert.SerializeObject(problem));
                }
                else
                {
                    _logger.LogWarning("Can't write error response. Response has already started.");
                }
            }
        }
    }
}
