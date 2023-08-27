using MediatR;

namespace Ates.Tracker.Domain.Events;

internal class TaskReassignedDomainEvent : INotification
{
    public TaskReassignedDomainEvent(Task task)
    {
        Task = task;
    }
    
    public Task Task { get; }
}