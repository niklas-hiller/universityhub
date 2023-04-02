namespace University.Server.Middlewares
{
    public static class Startup
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}