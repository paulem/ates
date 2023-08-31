using MediatR;

namespace Ates.Accounting.Application.IntegrationEvents;

public class TaskCreatedIntegrationEvent : INotification
{
    public TaskCreatedIntegrationEvent(Guid taskId, Guid assigneeId, string title, int fee, int reward, Domain.TaskStatus status)
    {
        TaskId = taskId;
        AssigneeId = assigneeId;
        Title = title;
        Fee = fee;
        Reward = reward;
        Status = status;
    }
    
    public Guid TaskId { get; }
    public Guid AssigneeId { get; }
    public string Title { get; set; }
    public int Fee { get; set; }
    public int Reward { get; set; }
    public Domain.TaskStatus Status { get; set; }
}