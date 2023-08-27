using MediatR;

namespace Ates.Tracker.Domain.Events;

internal class TaskCreatedDomainEvent : INotification
{
    public TaskCreatedDomainEvent(Task task)
    {
        Task = task;
    }
    
    public Task Task { get; }
}