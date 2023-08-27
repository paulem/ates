using Ates.Tracker.SeedWork;

namespace Ates.Tracker.Domain;

public class Assignment : Entity
{
    public Assignment(Account? currentAccount, Account newAccount, DateTime assignedAt)
    {
        CurrentAccount = currentAccount;
        NewAccount = newAccount;
        AssignedAt = assignedAt;
    }
    
    private Assignment() { }
    
    public Account? CurrentAccount { get; private set; }
    public Account NewAccount { get; private set; }
    public DateTime AssignedAt { get; private set; }
}