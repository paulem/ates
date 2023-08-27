namespace Ates.Accounting.Application.IntegrationEvents.Kafka;

public class KafkaEvent<T> where T : class
{
    public Guid EventId { get; }
    public string Name { get; set; }
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public T Data { get; set; }

    public KafkaEvent(Guid eventId, int version, DateTime createdAt, T data)
    {
        EventId = eventId;
        Name = GetEventName(data);
        Version = version;
        CreatedAt = createdAt;
        Data = data;
    }
    
    private static string GetEventName(T data)
    {
        var name = data.GetType().Name.Split("IntegrationEvent")[0];
        return name;
    }
}