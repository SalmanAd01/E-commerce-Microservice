using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_service.Dtos.Store
{
    public class UpdateStoreDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public required string StoreCode { get; set; }
        [Required]
        [MaxLength(200)]
        public required string Address { get; set; }
        [Required]
        [MaxLength(100)]
        public required string City { get; set; }
        [Required]
        [MaxLength(100)]
        public required string State { get; set; }
        [Required]
        [MaxLength(20)]
        public required string ZipCode { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Country { get; set; }
    }
}