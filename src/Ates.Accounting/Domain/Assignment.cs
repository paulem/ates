using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

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