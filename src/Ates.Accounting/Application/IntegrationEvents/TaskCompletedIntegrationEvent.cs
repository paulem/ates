using MediatR;

namespace Ates.Accounting.Application.IntegrationEvents;

public class TaskCompletedIntegrationEvent : INotification
{
    public TaskCompletedIntegrationEvent(Guid taskId, Guid assigneeId)
    {
        TaskId = taskId;
        AssigneeId = assigneeId;
    }
    
    public Guid TaskId { get; }
    public Guid AssigneeId { get; }
}