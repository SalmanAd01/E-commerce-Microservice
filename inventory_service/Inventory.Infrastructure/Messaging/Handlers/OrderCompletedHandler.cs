using System.Text.Json;
using Inventory.Application.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Handlers
{
    public class OrderCompletedHandler : IKafkaMessageHandler
    {
        public IEnumerable<string> Topics => new[] { "order.completed" };

        public async Task<IEnumerable<ProducedMessage>> HandleAsync(string payload, IServiceProvider services, CancellationToken cancellationToken)
        {
            var results = new List<ProducedMessage>();
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<OrderCompletedHandler>>();
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

                            bool committed = false;
                            if (repo != null && storeId > 0 && !string.IsNullOrEmpty(sku))
                            {
                                try
                                {
                                    committed = await repo.CommitReservationAsync(storeId, sku, qty, cancellationToken).ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "Error committing reservation for sku {sku} store {storeId}", sku, storeId);
                                    committed = false;
                                }
                            }
                            else
                            {
                                logger.LogWarning("Missing storeId or sku in order.completed for order {orderId}", orderId);
                            }

                            var outEvent = new Dictionary<string, object>
                            {
                                ["orderId"] = orderId!,
                                ["storeId"] = storeId,
                                ["productSku"] = sku,
                                ["quantity"] = qty,
                                ["status"] = committed ? "committed" : "commit_failed"
                            };
                            var msg = JsonSerializer.Serialize(outEvent);
                            results.Add(new ProducedMessage(committed ? "inventory.committed" : "inventory.commit_failed", msg));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to process item in order.completed");
                        }
                    }
                }
                else
                {
                    logger.LogWarning("order.completed received without items for order {orderId}", orderId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle order.completed payload");
            }

            return results;
        }
    }
}
