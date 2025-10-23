using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_catalog_service.Dtos.Product.Variant
{
    public class VariantDto
    {
        public required string Name { get; set; }
        public required string Sku { get; set; }
    }
}