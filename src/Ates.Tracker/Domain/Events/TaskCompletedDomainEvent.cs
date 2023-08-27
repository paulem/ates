using MediatR;

namespace Ates.Tracker.Domain.Events;

internal class TaskCompletedDomainEvent : INotification
{
    public TaskCompletedDomainEvent(Task task)
    {
        Task = task;
    }
    
    public Task Task { get; }
}