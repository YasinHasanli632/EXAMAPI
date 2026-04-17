using ExamAPI.Middlewares.Models;
using System.Text.Json;

namespace ExamAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception. Path: {Path}, Method: {Method}",
                    context.Request.Path,
                    context.Request.Method);

                await HandleExceptionAsync(context, ex, _environment.IsDevelopment());
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            bool isDevelopment)
        {
            var statusCode = exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ApiErrorResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = GetFriendlyMessage(exception, statusCode),
                Details = isDevelopment ? exception.ToString() : null,
                Errors = null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static string GetFriendlyMessage(Exception exception, int statusCode)
        {
            if (!string.IsNullOrWhiteSpace(exception.Message))
                return exception.Message;

            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Sorğuda xəta var.",
                StatusCodes.Status401Unauthorized => "Bu əməliyyat üçün icazəniz yoxdur.",
                StatusCodes.Status404NotFound => "Axtarılan məlumat tapılmadı.",
                _ => "Serverdə gözlənilməz xəta baş verdi."
            };
        }
    }
}
