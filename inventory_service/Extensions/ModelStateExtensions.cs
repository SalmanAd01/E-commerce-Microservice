using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inventory_service.Extensions
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