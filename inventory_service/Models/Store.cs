using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Models
{
    [Index(nameof(StoreCode), IsUnique = true)]
    [Index(nameof(Name), nameof(City), IsUnique = true)]
    [Table("stores")]
    public class Store
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(100)]
        public required string Name { get; set; }
        [Column("store_code")]
        [MaxLength(50)]
        public required string StoreCode { get; set; }
        [Column("address")]
        [MaxLength(200)]
        public required string Address { get; set; }
        [Column("city")]
        [MaxLength(100)]
        public required string City { get; set; }
        [Column("state")]
        [MaxLength(100)]
        public required string State { get; set; }
        [Column("zip_code")]
        [MaxLength(20)]
        public required string ZipCode { get; set; }
        [Column("country")]
        [MaxLength(100)]
        public required string Country { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}