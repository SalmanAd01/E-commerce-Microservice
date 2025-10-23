using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;
using inventory_service.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace inventory_service.Controllers.v1
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
        public async Task<IActionResult> GetStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreDto createDto)
        {
            var createdStore = await _storeService.CreateStoreAsync(createDto);
            return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, createdStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] UpdateStoreDto
    updateDto)
        {
            var updatedStore = await _storeService.UpdateStoreAsync(id, updateDto);
            if (updatedStore == null)
            {
                return NotFound();
            }
            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            await _storeService.DeleteStoreAsync(id);
            return NoContent();
        }
        
    }
}