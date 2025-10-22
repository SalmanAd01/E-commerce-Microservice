using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace product_catalog_service.Dtos.Brand
{
    public class UpdateBrandDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1)]
        public required string Description { get; set; }

        [Required]
        [Url]
        public required string Logo { get; set; }
    }
}