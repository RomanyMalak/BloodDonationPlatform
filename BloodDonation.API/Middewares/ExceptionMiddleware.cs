using System.Net;
using System.Text.Json;

namespace BloodDonation.API.Middewares
{
    /// <summary>
    /// Middleware that catches unhandled exceptions from downstream middleware or request handlers,
    /// logs the exception, and returns a standardized JSON error response to the client.
    /// </summary>
    /// <remarks>
    /// Register this middleware early in the pipeline to ensure it can handle exceptions from
    /// subsequent components. The middleware maps specific exception types (for example
    /// <see cref="UnauthorizedAccessException"/>) to appropriate HTTP status codes and messages.
    /// When the application environment is development, response details include the exception stack trace.
    /// </remarks>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Invokes the middleware for the current HTTP context.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A <see cref="Task"/> that represents the completion of request processing.</returns>
        /// <remarks>
        /// This middleware forwards the request to the next component in the pipeline and
        /// catches any unhandled exceptions. Caught exceptions are logged and converted into
        /// a JSON error response via <see cref="HandleExceptionAsync(HttpContext, Exception)"/>.
        /// </remarks>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles an exception by producing a standardized JSON error response.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> to write the response to.</param>
        /// <param name="exception">The <see cref="Exception"/> that occurred while processing the request.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
        /// <remarks>
        /// The response content type is set to "application/json". The method maps specific exceptions
        /// (for example <see cref="UnauthorizedAccessException"/>) to appropriate HTTP status codes and
        /// messages. In development environments, exception stack trace is included in the response details.
        /// </remarks>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "حدث خطأ غير متوقع في السيرفر، يرجى المحاولة لاحقاً.";

          
            if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                message = "غير مصرح لك بالوصول لهذا المورد.";
            }

            context.Response.StatusCode = (int)statusCode;

           
            var response = new CustomErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
               
                Details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null
            };

          
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }

  
    public class CustomErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}

