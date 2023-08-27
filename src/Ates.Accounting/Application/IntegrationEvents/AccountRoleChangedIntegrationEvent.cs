using Ates.Accounting.Domain;
using MediatR;

namespace Ates.Accounting.Application.IntegrationEvents;

public class AccountRoleChangedIntegrationEvent : INotification
{
    public AccountRoleChangedIntegrationEvent(Guid publicId, AccountRole role)
    {
        PublicId = publicId;
        Role = role;
    }
    
    public Guid PublicId { get; }
    public AccountRole Role { get; }
}