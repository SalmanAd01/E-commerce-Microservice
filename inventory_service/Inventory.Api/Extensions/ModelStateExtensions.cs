using Microsoft.AspNetCore.Mvc.ModelBinding;
using Inventory.Application.Common;

namespace Inventory.Api.Extensions
{
    public static class ModelStateExtensions
    {
        public static ErrorResponse ToErrorResponse(this ModelStateDictionary modelState)
        {
            var messages = modelState.Values.SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? string.Empty : e.ErrorMessage);

            var combined = string.Join(
                "; ",
                messages.Where(m => !string.IsNullOrWhiteSpace(m))
            );
            return new ErrorResponse(System.Net.HttpStatusCode.BadRequest, "Bad Request", string.IsNullOrWhiteSpace(combined) ? "Invalid request" : combined);
        }
    }
}
