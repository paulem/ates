using MediatR;

namespace Ates.Auth.Domain.Events;

internal class AccountRoleChangedDomainEvent : INotification
{
    public AccountRoleChangedDomainEvent(Account account, AccountRole oldRole)
    {
        Account = account;
        OldRole = oldRole;
    }
    
    public Account Account { get; }
    public AccountRole OldRole { get; }
}