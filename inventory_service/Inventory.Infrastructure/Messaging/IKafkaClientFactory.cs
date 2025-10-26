using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Inventory.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Inventory.Infrastructure.Messaging
{
    public interface IKafkaClientFactory
    {
        IConsumer<Ignore, string> CreateConsumer(string? groupId = null);
        IProducer<Null, string> CreateProducer();
        IAdminClient CreateAdminClient();
    }

    public class KafkaClientFactory : IKafkaClientFactory
    {
        private readonly KafkaOptions _options;
        public KafkaClientFactory(IOptions<KafkaOptions> options)
        {
            _options = options.Value;
        }

        public IAdminClient CreateAdminClient()
        {
            var adminConfig = new AdminClientConfig { BootstrapServers = _options.BootstrapServers };
            return new AdminClientBuilder(adminConfig).Build();
        }

        public IConsumer<Ignore, string> CreateConsumer(string? groupId = null)
        {
            var cfg = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = string.IsNullOrWhiteSpace(groupId) ? _options.GroupId : groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            return new ConsumerBuilder<Ignore, string>(cfg).Build();
        }

        public IProducer<Null, string> CreateProducer()
        {
            var producerConfig = new ProducerConfig { BootstrapServers = _options.BootstrapServers };
            return new ProducerBuilder<Null, string>(producerConfig).Build();
        }
    }
}
