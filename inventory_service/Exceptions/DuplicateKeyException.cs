using System;

namespace inventory_service.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public string? Column { get; }
        public string? Value { get; }

        public DuplicateKeyException(string message) : base(message)
        {
        }

        public DuplicateKeyException(string column, string value)
            : base($"{column} '{value}' already exists.")
        {
            Column = column;
            Value = value;
        }
    }
}
