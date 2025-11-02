using System.Threading.Tasks;
using Grpc.Core;
using Inventory.Application.Services.Contracts;
using Inventory.Grpc;

namespace Inventory.Api.Grpc
{
    public class InventoryPricingService : InventoryPricing.InventoryPricingBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryPricingService> _logger;

        public InventoryPricingService(IInventoryService inventoryService, ILogger<InventoryPricingService> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public override async Task<PriceResponse> GetSellingPrice(PriceRequest request, ServerCallContext context)
        {
            var item = await _inventoryService.GetByStoreAndProductSkuAsync((int)request.StoreId, request.ProductSku, context.CancellationToken);
            var response = new PriceResponse
            {
                SellingPrice = item?.SellingPrice is decimal d ? (double)d : 0d,
                Available = item is not null
            };

            if (!response.Available)
            {
                _logger.LogWarning("Inventory not found for storeId={StoreId}, productSku={Sku}", request.StoreId, request.ProductSku);
            }

            return response;
        }
    }
}
