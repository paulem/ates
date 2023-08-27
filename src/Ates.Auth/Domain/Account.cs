using Ates.Auth.Domain.Events;
using Ates.Auth.SeedWork;

namespace Ates.Auth.Domain;

public class Account : Entity
{
    public Account(Guid publicId, string email, AccountRole role, string passwordHash, DateTime createdAt)
    {
        PublicId = publicId;
        Email = email;
        Role = role;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;

        AddDomainEvent(new AccountCreatedDomainEvent(this));
    }
    
    public Guid PublicId { get; private set; }
    public AccountRole Role { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void ChangeRole(AccountRole newRole)
    {
        var oldRole = Role;
        Role = newRole;
        AddDomainEvent(new AccountRoleChangedDomainEvent(this, oldRole));
    }
}

public enum AccountRole
{
    Worker,
    Admin
}