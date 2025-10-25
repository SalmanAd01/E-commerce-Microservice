using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace inventory_service.Services
{
    public class KafkaHostedService : BackgroundService
    {
        private readonly ILogger<KafkaHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly IHostApplicationLifetime _appLifetime;

        public KafkaHostedService(ILogger<KafkaHostedService> logger, IServiceProvider services, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _services = services;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait until the host has fully started (Kestrel bound) before starting Kafka connections.
            // This avoids doing heavy network work during the host bind phase which can interfere with startup.
            try
            {
                // This Task will be canceled once the host signals ApplicationStarted.
                await Task.Delay(Timeout.Infinite, _appLifetime.ApplicationStarted).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // ApplicationStarted token cancelled -> host has started. Proceed to connect to Kafka.
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:7092",
                GroupId = "inventory-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            // Ensure topics exist (create if missing) before subscribing to avoid "Unknown topic or partition" errors
            var adminConfig = new AdminClientConfig { BootstrapServers = "localhost:7092" };
            try
            {
                using var adminClient = new AdminClientBuilder(adminConfig).Build();
                var topicsToEnsure = new[] { "order.created", "order.cancelled", "inventory.reserved", "inventory.reservation_failed", "inventory.released", "payment.initiated", "payment.succeeded", "payment.failed" };
                var specs = topicsToEnsure.Select(t => new TopicSpecification { Name = t, NumPartitions = 1, ReplicationFactor = 1 }).ToList();
                try
                {
                    adminClient.CreateTopicsAsync(specs).GetAwaiter().GetResult();
                }
                catch (CreateTopicsException ctex)
                {
                    foreach (var result in ctex.Results)
                    {
                        // ignore topics that already exist
                        if (result.Error.Code == ErrorCode.TopicAlreadyExists)
                        {
                            _logger.LogInformation("Topic {topic} already exists", result.Topic);
                        }
                        else
                        {
                            _logger.LogWarning("Create topic {topic} returned: {error}", result.Topic, result.Error.Reason);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to ensure Kafka topics exist; proceeding to subscribe (broker may auto-create topics)");
            }

            consumer.Subscribe(new[] { "order.created", "order.cancelled" });

            var producerConfig = new ProducerConfig { BootstrapServers = "localhost:7092" };
            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            _logger.LogInformation("Kafka consumer started for topics order.created and order.cancelled");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);
                        var payload = cr.Message.Value;
                        _logger.LogInformation("Received event on topic {topic}: {payload}", cr.Topic, payload);

                        // parse event
                        var evt = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
                        var orderId = evt != null && evt.ContainsKey("orderId") ? evt["orderId"].ToString() : null;
                        var amount = evt != null && evt.ContainsKey("totalAmount") && decimal.TryParse(evt["totalAmount"]?.ToString(), out var a) ? a : 0m;

                        // Use a scoped repository to operate on DB
                        using var scope = _services.CreateScope();
                        var repo = scope.ServiceProvider.GetService<inventory_service.Repositories.IInventoryRepository>();

                        if (cr.Topic == "order.created")
                        {
                            // Expect payload to include either a single item (storeId, productSku, quantity) or an "items" array.
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
                                // single item expected
                                items.Add(evt ?? new Dictionary<string, object>());
                            }

                            var reservationResults = new List<Dictionary<string, object>>();
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
                                        reserved = await repo.TryReserveAsync(storeId, sku, qty, stoppingToken).ConfigureAwait(false);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error reserving inventory for sku {sku} store {storeId}", sku, storeId);
                                        reserved = false;
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("Missing storeId or sku in order.created for order {orderId}", orderId);
                                }

                                var itemResult = new Dictionary<string, object>
                                {
                                    ["orderId"] = orderId,
                                    ["storeId"] = storeId,
                                    ["productSku"] = sku,
                                    ["quantity"] = qty,
                                    ["status"] = reserved ? "reserved" : "reservation_failed",
                                    ["amount"] = amount
                                };
                                reservationResults.Add(itemResult);

                                var outTopic = reserved ? "inventory.reserved" : "inventory.reservation_failed";
                                var msg = JsonSerializer.Serialize(itemResult);
                                await producer.ProduceAsync(outTopic, new Message<Null, string> { Value = msg }, stoppingToken);
                                _logger.LogInformation("Produced {topic} for order {orderId} sku {sku}", outTopic, orderId, sku);
                            }
                        }
                        else if (cr.Topic == "order.cancelled")
                        {
                            // When order cancelled, release inventory for provided items (if included)
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
                                                await repo.ReleaseReservationAsync(storeId, sku, qty, stoppingToken).ConfigureAwait(false);
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError(ex, "Error releasing reservation for sku {sku} store {storeId}", sku, storeId);
                                            }
                                        }

                                        var outEvent = new Dictionary<string, object>
                                        {
                                            ["orderId"] = orderId,
                                            ["storeId"] = storeId,
                                            ["productSku"] = sku,
                                            ["quantity"] = qty,
                                            ["status"] = "released"
                                        };
                                        var msg = JsonSerializer.Serialize(outEvent);
                                        await producer.ProduceAsync("inventory.released", new Message<Null, string> { Value = msg }, stoppingToken);
                                        _logger.LogInformation("Produced inventory.released for order {orderId} sku {sku}", orderId, sku);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Failed to process item in order.cancelled");
                                    }
                                }
                            }
                            else
                            {
                                // No items provided: cannot determine which reservations to release
                                _logger.LogWarning("order.cancelled received without items for order {orderId}", orderId);
                            }
                        }
                    }
                    catch (ConsumeException cex)
                    {
                        _logger.LogError(cex, "Kafka consume error");
                    }
                    catch (OperationCanceledException) { break; }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
