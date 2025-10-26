using System.Text.Json;
using Inventory.Application.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Handlers
{
    public class OrderCancelledHandler : IKafkaMessageHandler
    {
        public IEnumerable<string> Topics => new[] { "order.cancelled" };

        public async Task<IEnumerable<ProducedMessage>> HandleAsync(string payload, IServiceProvider services, CancellationToken cancellationToken)
        {
            var results = new List<ProducedMessage>();
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<OrderCancelledHandler>>();
            var repo = scope.ServiceProvider.GetService<IInventoryRepository>();

            try
            {
                var evt = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
                var orderId = evt != null && evt.ContainsKey("orderId") ? evt["orderId"]?.ToString() : null;

                if (evt != null && evt.ContainsKey("items") && evt["items"] is JsonElement jeArr && jeArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in jeArr.EnumerateArray())
                    {
                        try
                        {
                            var item = JsonSerializer.Deserialize<Dictionary<string, object>>(el.GetRawText());
                            int storeId = item != null && item.ContainsKey("storeId") && int.TryParse(item["storeId"]?.ToString(), out var s2) ? s2 : 0;
                            string sku = item != null && item.ContainsKey("productSku") ? item["productSku"]?.ToString() ?? string.Empty : string.Empty;
                            int qty = item != null && item.ContainsKey("quantity") && int.TryParse(item["quantity"]?.ToString(), out var q2) ? q2 : 1;
                            if (repo != null && storeId > 0 && !string.IsNullOrEmpty(sku))
                            {
                                try
                                {
                                    await repo.ReleaseReservationAsync(storeId, sku, qty, cancellationToken).ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "Error releasing reservation for sku {sku} store {storeId}", sku, storeId);
                                }
                            }

                            var outEvent = new Dictionary<string, object>
                            {
                                ["orderId"] = orderId!,
                                ["storeId"] = storeId,
                                ["productSku"] = sku,
                                ["quantity"] = qty,
                                ["status"] = "released"
                            };
                            var msg = JsonSerializer.Serialize(outEvent);
                            results.Add(new ProducedMessage("inventory.released", msg));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to process item in order.cancelled");
                        }
                    }
                }
                else
                {
                    logger.LogWarning("order.cancelled received without items for order {orderId}", orderId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle order.cancelled payload");
            }

            return results;
        }
    }
}
