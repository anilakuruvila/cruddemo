using Assessment.Api.Exceptions;
using System.Net;
using System.Text.Json;

namespace Assessment.Api.Utilities
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            var exceptionType = exception.GetType();

            if (exceptionType == typeof(UniqueException))
            {
                message = exception.Message;
                status = HttpStatusCode.InternalServerError;
            }
            else if (exceptionType == typeof(NotFoundException))
            {
                message = exception.Message;
                status = HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(RequiredException))
            {
                message = exception.Message;
                status = HttpStatusCode.PreconditionFailed;
            }
            else if (exceptionType == typeof(MaxLengthException))
            {
                message = exception.Message;
                status = HttpStatusCode.PreconditionFailed;
            }
            else
            {
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
            }

            var exceptionResult = JsonSerializer.Serialize(new { error = message, status });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
