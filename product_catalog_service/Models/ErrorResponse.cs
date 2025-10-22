using System;
using System.Net;

namespace product_catalog_service.Models
{
    // Error response returned by the global exception handler
    public class ErrorResponse
    {
        public DateTime Timestamp { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ErrorResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public ErrorResponse(HttpStatusCode status, string error, string message) : this()
        {
            Status = status;
            Error = error;
            Message = message;
        }
    }
}
