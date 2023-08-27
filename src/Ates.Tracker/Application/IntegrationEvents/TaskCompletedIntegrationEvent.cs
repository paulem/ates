namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskCompletedIntegrationEvent
{
    public TaskCompletedIntegrationEvent(Guid publicId)
    {
        PublicId = publicId;
    }
    
    public Guid PublicId { get; }
}