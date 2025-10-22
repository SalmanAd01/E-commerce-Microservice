using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace product_catalog_service.Extensions
{
    public static class ModelStateExtensions
    {
        public static Models.ErrorResponse ToErrorResponse(this ModelStateDictionary modelState)
        {
            var messages = modelState.Values.SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message ?? string.Empty : e.ErrorMessage);

            var combined = string.Join("; ", messages.Where(m => !string.IsNullOrWhiteSpace(m)));
            return new Models.ErrorResponse(System.Net.HttpStatusCode.BadRequest, "Bad Request", string.IsNullOrWhiteSpace(combined) ? "Invalid request" : combined);
        }
    }
}
