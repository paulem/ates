using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MediatR;

namespace Ates.Tracker.Application.IntegrationEvents.Kafka;

public interface IKafkaConsumerMessageHandler
{
    Task Handle(string topic, string message);
}

public class KafkaConsumerMessageHandler : IKafkaConsumerMessageHandler
{
    private readonly IPublisher _publisher;

    public KafkaConsumerMessageHandler(IPublisher publisher)
    {
        _publisher = publisher;
    }
    
    public async Task Handle(string topic, string message)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() }};
        var kafkaEvent = JsonSerializer.Deserialize<KafkaEvent<JsonObject>>(message, jsonSerializerOptions);
        
        ArgumentNullException.ThrowIfNull(kafkaEvent);

        INotification? @event = kafkaEvent.Name switch
        {
            "AccountCreated" when kafkaEvent.Version == 1 =>
                kafkaEvent.Data.Deserialize<AccountCreatedIntegrationEvent>(jsonSerializerOptions),
            "AccountRoleChanged" when kafkaEvent.Version == 1 => kafkaEvent.Data
                .Deserialize<AccountRoleChangedIntegrationEvent>(jsonSerializerOptions),
            
            _ => throw new NotSupportedException($"Unsupported Kafka event \"{kafkaEvent.Name}\" or event version \"{kafkaEvent.Version}\".")
        };

        // Publish

        if (@event is not null)
        {
            if (topic == "accounts")
                await _publisher.Publish(@event);
            
            if (topic == "accounts-streaming")
                await _publisher.Publish(@event);
        }

        await Task.CompletedTask;
    }
}