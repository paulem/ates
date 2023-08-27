using Confluent.Kafka;

namespace Ates.Accounting.Application.IntegrationEvents.Kafka;

public class KafkaProducerOptions
{
    public string BootstrapServers { get; init; }
    public SecurityProtocol SecurityProtocol { get; init; }
    public SaslMechanism SaslMechanism { get; init; }
    public string SaslUsername { get; init; }
    public string SaslPassword { get; init; }

}