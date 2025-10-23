using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using product_catalog_service.Services;
using product_catalog_service.Dtos.Product;
using product_catalog_service.Dtos.Template;
using MongoDB.Bson;

namespace product_catalog_service.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IProductService productService, IBrandService brandService, IHttpClientFactory httpClientFactory)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

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

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var created = await _productService.CreateProductAsync(dto, cancellationToken).ConfigureAwait(false);
                var resp = Mappers.ProductResponseMapper.ToResponse(created);
                return CreatedAtAction(nameof(GetProductById), new { id = created.Id }, resp);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}