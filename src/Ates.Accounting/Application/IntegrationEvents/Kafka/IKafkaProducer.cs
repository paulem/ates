namespace Ates.Accounting.Application.IntegrationEvents.Kafka;

public interface IKafkaProducer : IDisposable
{
    Task Produce(string topic, string message, CancellationToken cancellationToken);
}