using System;

namespace ProductCatalog.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class MongoIndexAttribute : Attribute
    {
        public bool Unique { get; set; }
        public string? Name { get; set; }

        public MongoIndexAttribute()
        {
            Unique = false;
        }
    }
}
