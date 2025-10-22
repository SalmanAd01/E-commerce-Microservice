using System;
using MongoDB.Bson;

namespace product_catalog_service.Services
{
    // Helper to convert an input string and a template DataType name into a BsonValue
    // Template service stores enum names as strings: "TEXT", "NUMBER", "BOOLEAN"
    public static class AttributeValueMapper
    {
        public static BsonValue? Map(string? dataTypeName, string? rawValue)
        {
            if (rawValue == null)
                return null;

            if (string.IsNullOrWhiteSpace(dataTypeName))
            {
                // fallback to text
                return new BsonString(rawValue);
            }

            switch (dataTypeName.Trim().ToUpperInvariant())
            {
                case "TEXT":
                    return new BsonString(rawValue);

                case "NUMBER":
                    // Try int, long, double, decimal128 (in that order)
                    if (Int32.TryParse(rawValue, out var i))
                        return new BsonInt32(i);
                    if (Int64.TryParse(rawValue, out var l))
                        return new BsonInt64(l);
                    if (Double.TryParse(rawValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                        return new BsonDouble(d);
                    // As a last resort, attempt Decimal128
                    if (Decimal.TryParse(rawValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dec))
                        return new BsonDecimal128(Decimal128.Parse(rawValue));

                    // if parsing failed, store original string to avoid data loss
                    return new BsonString(rawValue);

                case "BOOLEAN":
                    if (Boolean.TryParse(rawValue, out var b))
                        return new BsonBoolean(b);
                    // also accept 0/1
                    if (rawValue == "0") return new BsonBoolean(false);
                    if (rawValue == "1") return new BsonBoolean(true);
                    // fallback to string
                    return new BsonString(rawValue);

                default:
                    // Unknown data type name, store raw string
                    return new BsonString(rawValue);
            }
        }
    }
}
