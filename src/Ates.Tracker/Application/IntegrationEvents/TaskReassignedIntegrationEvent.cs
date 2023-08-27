namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskReassignedIntegrationEvent
{
    public TaskReassignedIntegrationEvent(Guid publicId)
    {
        PublicId = publicId;
    }
    
    public Guid PublicId { get; }
}