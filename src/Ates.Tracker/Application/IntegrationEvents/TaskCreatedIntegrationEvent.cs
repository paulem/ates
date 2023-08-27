namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskCreatedIntegrationEvent
{
    public TaskCreatedIntegrationEvent(Guid publicId)
    {
        PublicId = publicId;
    }
    
    public Guid PublicId { get; }
}