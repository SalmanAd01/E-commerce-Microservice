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
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync().ConfigureAwait(false);
            var resp = products.Select(Mappers.ProductResponseMapper.ToResponse);
            return Ok(resp);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id).ConfigureAwait(false);
            if (product == null) return NotFound();

            var resp = Mappers.ProductResponseMapper.ToResponse(product);
            return Ok(resp);
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

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProduct(string id, [FromBody] CreateProductDto dto, CancellationToken cancellationToken)
            {
                try
                {
                    var updated = await _productService.UpdateProductAsync(id, dto, cancellationToken).ConfigureAwait(false);
                    var resp = Mappers.ProductResponseMapper.ToResponse(updated);
                    return Ok(resp);
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

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProduct(string id, CancellationToken cancellationToken)
            {
                try
                {
                    await _productService.DeleteProductAsync(id, cancellationToken).ConfigureAwait(false);
                    return NoContent();
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