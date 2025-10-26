using System;
using System.Threading.Tasks;
using Inventory.Application.Dtos.Inventory;
using Inventory.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/inventories")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var items = await _inventoryService.GetAllAsync(cancellationToken);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _inventoryService.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInventoryDto createDto, CancellationToken cancellationToken)
        {
            var created = await _inventoryService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInventoryDto updateDto, CancellationToken cancellationToken)
        {
            var updated = await _inventoryService.UpdateAsync(id, updateDto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _inventoryService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("store/{storeId}/productsku/{productSku}")]
        public async Task<IActionResult> GetByStoreAndProductSku(int storeId, string productSku, CancellationToken cancellationToken)
        {
            var item = await _inventoryService.GetByStoreAndProductSkuAsync(storeId, productSku, cancellationToken);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}
