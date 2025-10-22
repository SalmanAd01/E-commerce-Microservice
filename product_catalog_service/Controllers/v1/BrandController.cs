using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using product_catalog_service.Dtos.Brand;
using product_catalog_service.Mappers;
using product_catalog_service.Models;
using product_catalog_service.Repositories;
using product_catalog_service.Services;

namespace product_catalog_service.Controllers
{
    [ApiController]
    [Route("api/v1/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        }

        [HttpGet]
        public async Task<ActionResult<List<BrandDto>>> GetBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync().ConfigureAwait(false);
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrandById(string id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id).ConfigureAwait(false);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto brand)
        {
            var createdBrand = await _brandService.CreateBrandAsync(brand).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetBrandById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BrandDto>> UpdateBrand(string id, [FromBody] UpdateBrandDto brand)
        {
            var updated = await _brandService.UpdateBrandAsync(id, brand).ConfigureAwait(false);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(string id)
        {
            await _brandService.DeleteBrandAsync(id).ConfigureAwait(false);
            return NoContent();
        }
    }
}