using System.Net;
using Inventory.Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.ExceptionHandlers
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<CustomExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (httpContext == null) return false;

            exception ??= new Exception("Unknown error");

            _logger.LogError(exception, "Unhandled exception caught by {handler}", nameof(CustomExceptionHandler));
            HttpStatusCode status;
            string error;

            switch (exception)
            {
                case DbUpdateException dbUpdateEx when dbUpdateEx.InnerException != null:
                    {
                        var raw = dbUpdateEx.InnerException.Message ?? string.Empty;
                        await WriteErrorResponseAsync(httpContext, HttpStatusCode.Conflict, "Conflict", raw, cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    error = "Not Found";
                    break;
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    error = "Bad Request";
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    error = "Internal Server Error";
                    break;
            }
            var message = exception?.Message ?? string.Empty;
            await WriteErrorResponseAsync(httpContext, status, error, message, cancellationToken).ConfigureAwait(false);

            return true;
        }

        private static async Task WriteErrorResponseAsync(HttpContext httpContext, HttpStatusCode status, string error, string message, CancellationToken cancellationToken)
        {
            var response = new ErrorResponse(status, error, message ?? string.Empty);
            httpContext.Response.StatusCode = (int)status;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
