using Confluent.Kafka;

namespace Ates.Tracker.Application.IntegrationEvents.Kafka;

public class KafkaConsumerOptions
{
    public string BootstrapServers { get; init; }
    public SecurityProtocol SecurityProtocol { get; init; }
    public SaslMechanism SaslMechanism { get; init; }
    public string SaslUsername { get; init; }
    public string SaslPassword { get; init; }
    public string GroupId { get; init; }
    
    public string[] Topics { get; init; }
}