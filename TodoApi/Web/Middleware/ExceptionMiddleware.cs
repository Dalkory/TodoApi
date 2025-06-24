using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TodoApi.Core.Exceptions;

namespace TodoApi.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails();

            switch (exception)
            {
                case NotFoundException notFoundException:
                    problemDetails.Title = "Not Found";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = notFoundException.Message;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    break;

                case ArgumentException argumentException:
                    problemDetails.Title = "Bad Request";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = argumentException.Message;
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;

                default:
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = _env.IsDevelopment() ? exception.ToString() : "An error occurred while processing your request.";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                    break;
            }

            context.Response.StatusCode = problemDetails.Status.Value;
            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}