using MediatR;

namespace Ates.Accounting.Application.IntegrationEvents;

public class TaskReassignedIntegrationEvent : INotification
{
    public TaskReassignedIntegrationEvent(Guid taskId, Guid newAssigneeId)
    {
        TaskId = taskId;
        NewAssigneeId = newAssigneeId;
    }
    
    public Guid TaskId { get; }
    public Guid NewAssigneeId { get; }
}