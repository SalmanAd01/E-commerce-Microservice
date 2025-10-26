using System.Net;

namespace Inventory.Application.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; }
        public string Error { get; }
        public string Message { get; }

        public ErrorResponse(HttpStatusCode status, string error, string message)
        {
            StatusCode = (int)status;
            Error = error;
            Message = message;
        }
    }
}
