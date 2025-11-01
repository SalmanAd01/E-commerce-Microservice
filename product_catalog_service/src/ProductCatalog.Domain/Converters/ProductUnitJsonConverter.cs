using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Converters
{
    public class ProductUnitJsonConverter : JsonConverter<ProductUnit?>
    {
        public override ProductUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string? raw = null;

            if (reader.TokenType == JsonTokenType.String)
            {
                raw = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var intVal))
                {
                    if (Enum.IsDefined(typeof(ProductUnit), intVal))
                        return (ProductUnit)intVal;
                }
                raw = reader.GetDouble().ToString();
            }
            else
            {
                throw CreateFriendlyException("unit", reader.TokenType.ToString());
            }

            if (!string.IsNullOrWhiteSpace(raw))
            {
                // Try case-insensitive parse
                if (Enum.TryParse<ProductUnit>(raw, ignoreCase: true, out var parsed))
                {
                    return parsed;
                }
            }

            throw CreateFriendlyException("unit", raw);
        }

        public override void Write(Utf8JsonWriter writer, ProductUnit? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString());
            }
            else
            {
                writer.WriteNullValue();
            }
        }

        private static JsonException CreateFriendlyException(string field, string? provided)
        {
            var allowed = string.Join(", ", Enum.GetNames(typeof(ProductUnit)));
            var providedText = string.IsNullOrWhiteSpace(provided) ? "(null)" : provided;
            var message = $"Invalid value for '{field}': '{providedText}'. Allowed values: {allowed}.";
            return new JsonException(message);
        }
    }
}
