using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ates.Accounting.Application.IntegrationEvents.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(IOptions<KafkaProducerOptions> kafkaProducerOptions)
    {
        var producerOptions = kafkaProducerOptions.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = producerOptions.BootstrapServers,
            SecurityProtocol = producerOptions.SecurityProtocol,
            SaslMechanism = producerOptions.SaslMechanism,
            SaslUsername = producerOptions.SaslUsername,
            SaslPassword = producerOptions.SaslPassword
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task Produce(string topic, string message, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic, nameof(topic));
        ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
        
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message }, cancellationToken);
    }

    public void Dispose() => _producer.Dispose();
}