using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_catalog_service.Dtos.Brand
{
    public class CreateBrandDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Logo { get; set; }
    }
}