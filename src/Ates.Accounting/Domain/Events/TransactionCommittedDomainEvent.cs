using MediatR;

namespace Ates.Accounting.Domain.Events;

internal class TransactionCommittedDomainEvent : INotification
{
    public TransactionCommittedDomainEvent(Account account, Transaction transaction)
    {
        Account = account;
        Transaction = transaction;
    }
    
    public Account Account { get; }
    public Transaction Transaction { get; }
}