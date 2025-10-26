using System;
using System.Threading.Tasks;
using Inventory.Application.Dtos.Store;
using Inventory.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/stores")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
        }

        [HttpGet]
        public async Task<IActionResult> GetStores(CancellationToken cancellationToken)
        {
            var stores = await _storeService.GetAllStoresAsync(cancellationToken);
            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetStoreByIdAsync(id, cancellationToken);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreDto createDto, CancellationToken cancellationToken)
        {
            var createdStore = await _storeService.CreateStoreAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, createdStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreDto updateDto, CancellationToken cancellationToken)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(id, updateDto, cancellationToken);
            if (updatedStore == null)
            {
                return NotFound();
            }
            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id, CancellationToken cancellationToken)
        {
            await _storeService.DeleteStoreAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
