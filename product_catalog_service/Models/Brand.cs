using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace product_catalog_service.Models
{
    public class Brand
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("logo")]
        public required string Logo { get; set; }
    }
}