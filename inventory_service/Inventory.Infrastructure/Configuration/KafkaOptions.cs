namespace Inventory.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; } = "localhost:7092";
        public string GroupId { get; set; } = "inventory-service-group";
        public string[] EnsureTopics { get; set; } = new[] { "inventory.reserved", "inventory.reservation_failed", "inventory.released" };
    }
}
