using System.Text.Json;
using Inventory.Application.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Messaging.Handlers
{
    public class OrderCreatedHandler : IKafkaMessageHandler
    {
        public IEnumerable<string> Topics => new[] { "order.created" };

        public async Task<IEnumerable<ProducedMessage>> HandleAsync(string payload, IServiceProvider services, CancellationToken cancellationToken)
        {
            var results = new List<ProducedMessage>();
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<OrderCreatedHandler>>();
            var repo = scope.ServiceProvider.GetService<IInventoryRepository>();

            try
            {
                var evt = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
                var orderId = evt != null && evt.ContainsKey("orderId") ? evt["orderId"]?.ToString() : null;
                var amount = evt != null && evt.ContainsKey("totalAmount") && decimal.TryParse(evt["totalAmount"]?.ToString(), out var a) ? a : 0m;

                List<Dictionary<string, object>> items = new();
                if (evt != null && evt.ContainsKey("items") && evt["items"] is JsonElement je && je.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in je.EnumerateArray())
                    {
                        try
                        {
                            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(el.GetRawText());
                            if (dict != null) items.Add(dict);
                        }
                        catch { }
                    }
                }
                else
                {
                    items.Add(evt ?? new Dictionary<string, object>());
                }

                foreach (var item in items)
                {
                    int storeId = item.ContainsKey("storeId") && int.TryParse(item["storeId"]?.ToString(), out var s) ? s : 0;
                    string sku = item.ContainsKey("productSku") ? item["productSku"]?.ToString() ?? string.Empty : string.Empty;
                    int qty = item.ContainsKey("quantity") && int.TryParse(item["quantity"]?.ToString(), out var q) ? q : 1;

                    bool reserved = false;
                    if (repo != null && storeId > 0 && !string.IsNullOrEmpty(sku))
                    {
                        try
                        {
                            reserved = await repo.TryReserveAsync(storeId, sku, qty, cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error reserving inventory for sku {sku} store {storeId}", sku, storeId);
                            reserved = false;
                        }
                    }
                    else
                    {
                        logger.LogWarning("Missing storeId or sku in order.created for order {orderId}", orderId);
                    }

                    var itemResult = new Dictionary<string, object>
                    {
                        ["orderId"] = orderId!,
                        ["storeId"] = storeId,
                        ["productSku"] = sku,
                        ["quantity"] = qty,
                        ["status"] = reserved ? "reserved" : "reservation_failed",
                        ["amount"] = amount
                    };

                    var outTopic = reserved ? "inventory.reserved" : "inventory.reservation_failed";
                    var msg = JsonSerializer.Serialize(itemResult);
                    results.Add(new ProducedMessage(outTopic, msg));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to handle order.created payload");
            }

            return results;
        }
    }
}
