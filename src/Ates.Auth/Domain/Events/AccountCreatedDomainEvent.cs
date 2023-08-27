using MediatR;

namespace Ates.Auth.Domain.Events;

internal class AccountCreatedDomainEvent : INotification
{
    public AccountCreatedDomainEvent(Account account)
    {
        Account = account;
    }
    
    public Account Account { get; }
}