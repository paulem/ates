using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

public class Task : Entity
{
    private readonly List<Assignment> _assignments = new();

    public Task(Guid publicId, string title, string description, Account assignee, TaskPrice price, DateTime submittedAt)
    {
        CanBeAssignedTo(assignee, true);

        PublicId = publicId;
        Title = title;
        Description = description;
        Assignee = assignee;
        Price = price;
    
        Status = TaskStatus.Open;
        _assignments.Add(new Assignment(null, assignee, submittedAt));
        
        //AddDomainEvent(new TaskCreatedDomainEvent(this));
    }
    
    private Task() { }

    public Guid PublicId { get; }
    public string Title { get; }
    public string Description { get; }
    public TaskPrice Price { get; }
    public TaskStatus Status { get; private set; }
    public Account Assignee { get; private set; }
    
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    public static bool CanBeAssignedTo(Account account, bool throwDomainExceptionIfNot)
    {
        var canBeAssignee = account.Role == AccountRole.Worker;

        if (canBeAssignee == false && throwDomainExceptionIfNot)
            throw new DomainException("Only worker account can be assigned to task.");

        return canBeAssignee;
    }

    public void Reassign(Account newAssignee, DateTime reassignedAt)
    {
        CanBeAssignedTo(newAssignee, true);

        var current = Assignee;
        Assignee = newAssignee;
        _assignments.Add(new Assignment(current, newAssignee, reassignedAt));
        
        //AddDomainEvent(new TaskReassignedDomainEvent(this));
    }

    public void Complete()
    {
        if (Status == TaskStatus.Done)
            throw new DomainException("Task already completed. It cannot be completed twice.");

        Status = TaskStatus.Done;
        //AddDomainEvent(new TaskCompletedDomainEvent(this));
    }
}