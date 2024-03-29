namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskCompletedIntegrationEvent
{
    public TaskCompletedIntegrationEvent(Guid taskId, Guid assigneeId)
    {
        TaskId = taskId;
        AssigneeId = assigneeId;
    }
    
    public Guid TaskId { get; }
    public Guid AssigneeId { get; }
}