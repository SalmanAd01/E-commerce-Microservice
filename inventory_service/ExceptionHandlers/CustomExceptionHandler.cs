using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using inventory_service.Exceptions;
using inventory_service.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace inventory_service.ExceptionHandlers
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
                case DuplicateKeyException dkex:
                    {
                        var resp = new ErrorResponse(HttpStatusCode.Conflict, "Conflict", dkex.Message);
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        httpContext.Response.ContentType = "application/json";
                        await httpContext.Response.WriteAsJsonAsync(resp, cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                case KeyNotFoundException _:
                    status = HttpStatusCode.NotFound;
                    error = "Not Found";
                    break;
                case ArgumentException _:
                    status = HttpStatusCode.BadRequest;
                    error = "Bad Request";
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    error = "Internal Server Error";
                    break;
            }
            var response = new ErrorResponse(status, error, exception?.Message ?? string.Empty);

            httpContext.Response.StatusCode = (int)status;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);

            return true;
        }
    }
}