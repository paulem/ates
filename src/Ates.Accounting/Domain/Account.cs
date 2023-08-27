using Ates.Accounting.Domain.Events;
using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

public class Account : Entity
{
    private readonly List<Transaction> _transactions = new();

    public Account(Guid publicId, string email, AccountRole role)
    {
        PublicId = publicId;
        Email = email;
        Role = role;
    }
    
    public Guid PublicId { get; private set; }
    public AccountRole Role { get; private set; }
    public string Email { get; private set; }
    public int Balance { get; private set; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    
    public void ChangeRole(AccountRole newRole)
    {
        Role = newRole;
    }
    
    public Transaction DepositCompletionReward(int amount, string message)
    {
        var transaction = new Transaction(Guid.NewGuid(), amount, 0, message);
        _transactions.Add(transaction);
        
        Balance += amount;
        AddDomainEvent(new TransactionCommittedDomainEvent(this, transaction));

        return transaction;
    }
    
    public Transaction ChargeAssignmentFee(int amount, string message)
    {
        var transaction = new Transaction(Guid.NewGuid(), 0, amount, message);
        _transactions.Add(transaction);
        
        Balance -= amount;
        AddDomainEvent(new TransactionCommittedDomainEvent(this, transaction));
        
        return transaction;
    }
}

public enum AccountRole
{
    Worker,
    Admin
}