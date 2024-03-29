namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskReassignedIntegrationEvent
{
    public TaskReassignedIntegrationEvent(Guid taskId, Guid newAssigneeId)
    {
        TaskId = taskId;
        NewAssigneeId = newAssigneeId;
    }
    
    public Guid TaskId { get; }
    public Guid NewAssigneeId { get; }
}