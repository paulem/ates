using Ates.Auth.Domain;

namespace Ates.Auth.Application.IntegrationEvents;

public class AccountRoleChangedIntegrationEvent
{
    public AccountRoleChangedIntegrationEvent(Guid publicId, AccountRole role)
    {
        PublicId = publicId;
        Role = role;
    }
    
    public Guid PublicId { get; }
    public AccountRole Role { get; }
}