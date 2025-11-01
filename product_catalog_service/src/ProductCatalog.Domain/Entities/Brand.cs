using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Entities
{
    public class Brand
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Name { get; set; }

        [BsonElement("description")]
        [Required]
        [StringLength(250, MinimumLength = 1)]
        public required string Description { get; set; }

        [BsonElement("logo")]
        [Required]
        [Url]
        public required string Logo { get; set; }
    }
}
