using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Inventory.Application.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Inventory.Infrastructure.Messaging.Handlers;
using Inventory.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Inventory.Infrastructure.Messaging
{
    public class KafkaHostedService : BackgroundService
    {
    private readonly ILogger<KafkaHostedService> _logger;
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _appLifetime;
        private readonly IOptions<KafkaOptions> _options;
        private readonly IEnumerable<IKafkaMessageHandler> _handlers;
        private readonly IKafkaClientFactory _factory;

        public KafkaHostedService(ILogger<KafkaHostedService> logger, IServiceProvider services, IHostApplicationLifetime appLifetime, IOptions<KafkaOptions> options, IEnumerable<IKafkaMessageHandler> handlers, IKafkaClientFactory factory)
        {
            _logger = logger;
            _services = services;
            _appLifetime = appLifetime;
            _options = options;
            _handlers = handlers;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, _appLifetime.ApplicationStarted).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            using var consumer = _factory.CreateConsumer();
            try
            {
                using var adminClient = _factory.CreateAdminClient();
                var inboundTopics = _handlers.SelectMany(h => h.Topics).Distinct().ToArray();
                var outTopics = _options.Value.EnsureTopics ?? Array.Empty<string>();
                var topicsToEnsure = inboundTopics.Concat(outTopics).Distinct().ToArray();
                var specs = topicsToEnsure.Select(t => new TopicSpecification { Name = t, NumPartitions = 1, ReplicationFactor = 1 }).ToList();
                try
                {
                    adminClient.CreateTopicsAsync(specs).GetAwaiter().GetResult();
                }
                catch (CreateTopicsException ctex)
                {
                    foreach (var result in ctex.Results)
                    {
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

            var subscribedTopics = _handlers.SelectMany(h => h.Topics).Distinct().ToArray();
            consumer.Subscribe(subscribedTopics);

            using var producer = _factory.CreateProducer();

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

                        var handler = _handlers.FirstOrDefault(h => h.Topics.Contains(cr.Topic));
                        if (handler == null)
                        {
                            _logger.LogWarning("No handler registered for topic {topic}", cr.Topic);
                            continue;
                        }
                        var produced = await handler.HandleAsync(payload, _services, stoppingToken).ConfigureAwait(false);
                        foreach (var pm in produced)
                        {
                            await producer.ProduceAsync(pm.Topic, new Message<Null, string> { Value = pm.Payload }, stoppingToken);
                            _logger.LogInformation("Produced {topic} event", pm.Topic);
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
