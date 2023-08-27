using Ates.Accounting.Domain;
using MediatR;

namespace Ates.Accounting.Application.IntegrationEvents;

public class AccountCreatedIntegrationEvent : INotification
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