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
    [Route("api/v1/[controller]")]
    public class BrandController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;

        [HttpGet]
        public IActionResult GetBrands()
        {
            var brands = _brandService.GetAllBrandsAsync().Result;
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public IActionResult GetBrandById(string id)
        {
            var brand = _brandService.GetBrandByIdAsync(id).Result;
            if (brand == null)
            {
                return NotFound();
            }
            return Ok(brand);
        }

        [HttpPost]
        public IActionResult CreateBrand([FromBody] CreateBrandDto brand)
        {
            var createdBrand =  _brandService.CreateBrandAsync(brand).Result;
            return CreatedAtAction(nameof(GetBrandById), new { id = createdBrand.Id }, createdBrand);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBrand(string id, [FromBody] UpdateBrandDto brand)
        {
            _brandService.UpdateBrandAsync(id, brand);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(string id)
        {
            _brandService.DeleteBrandAsync(id);
            return NoContent();
        }
    }
}