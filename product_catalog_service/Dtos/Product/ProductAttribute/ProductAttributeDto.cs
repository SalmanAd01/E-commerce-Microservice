using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_catalog_service.Dtos.Product.ProductAttribute
{
    public class ProductAttributeDto
    {
        public required int AttributeId { get; set; }
        public required object Value { get; set; }
    }
}