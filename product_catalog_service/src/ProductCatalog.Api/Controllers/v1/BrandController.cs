using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Dtos.Brand;
using ProductCatalog.Application.Services;

namespace ProductCatalog.Api.Controllers
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
