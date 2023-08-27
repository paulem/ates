using Ates.Auth.Domain;

namespace Ates.Auth.Application.IntegrationEvents;

public class AccountCreatedIntegrationEvent
{
    public AccountCreatedIntegrationEvent(Guid publicId, string email, AccountRole role)
    {
        PublicId = publicId;
        Email = email;
        Role = role;
    }
    
    public Guid PublicId { get; }
    public string Email { get; }
    public AccountRole Role { get; }

}