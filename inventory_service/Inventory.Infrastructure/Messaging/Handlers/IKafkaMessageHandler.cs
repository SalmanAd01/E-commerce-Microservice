using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure.Messaging.Handlers
{
    public record ProducedMessage(string Topic, string Payload);

    public interface IKafkaMessageHandler
    {
        IEnumerable<string> Topics { get; }
        Task<IEnumerable<ProducedMessage>> HandleAsync(string payload, IServiceProvider services, CancellationToken cancellationToken);
    }
}
