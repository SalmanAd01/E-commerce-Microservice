using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace product_catalog_service.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController: ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(new { Message = "Product API is working!" });
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            return Ok(new { Message = $"Product API is working for product {id}!" });
        }
    }
}