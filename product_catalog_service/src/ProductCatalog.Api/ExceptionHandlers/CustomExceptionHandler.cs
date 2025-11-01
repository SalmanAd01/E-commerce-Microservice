using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;
using ProductCatalog.Domain.Common;

namespace ProductCatalog.Api.ExceptionHandlers
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (httpContext == null) return false;

            HttpStatusCode status;
            string error;

            switch (exception)
            {
                case MongoWriteException _:
                    status = HttpStatusCode.Conflict;
                    error = "Conflict";
                    break;
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
