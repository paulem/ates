namespace Ates.Tracker.Application.IntegrationEvents;

public class TaskCreatedIntegrationEvent
{
    public TaskCreatedIntegrationEvent(Guid taskId, Guid assigneeId, string title, string description, int fee, int reward, Domain.TaskStatus status)
    {
        TaskId = taskId;
        AssigneeId = assigneeId;
        Title = title;
        Description = description;
        Fee = fee;
        Reward = reward;
        Status = status;
    }
    
    public Guid TaskId { get; }
    public Guid AssigneeId { get; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public int Fee { get; set; }
    public int Reward { get; set; }
    public Domain.TaskStatus Status { get; set; }
}