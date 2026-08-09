using AuthenticationAPI.Shared.Middlewares;

namespace AuthenticationAPI.Shared.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Configures the application to use a series of predefined middleware components: CorrelationIdMiddleware, RequestLoggingMiddleware, PerformanceMiddleware, and ExceptionHandlingMiddleware.
        /// </summary>
        /// <param name="application">The <see cref="IApplicationBuilder"/> instance to configure the middleware pipeline.</param>
        /// <returns>The same <see cref="IApplicationBuilder"/> instance with the middleware configured in the pipeline.</returns>
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder application)
        {
            application.UseMiddleware<CorrelationIdMiddleware>();
            application.UseMiddleware<RequestLoggingMiddleware>();
            application.UseMiddleware<PerformanceMiddleware>();
            application.UseMiddleware<ExceptionHandlingMiddleware>();

            return application;
        }
    }
}