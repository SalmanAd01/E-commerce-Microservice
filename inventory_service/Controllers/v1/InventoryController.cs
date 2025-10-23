using System;
using System.Threading.Tasks;
using inventory_service.Dtos.Inventory;
using inventory_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventory_service.Controllers.v1
{
    [ApiController]
    [Route("api/v1/inventories")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _inventoryService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInventoryDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _inventoryService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _inventoryService.UpdateAsync(id, updateDto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
