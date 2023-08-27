using Ates.Accounting.SeedWork;

namespace Ates.Accounting.Domain;

public class Account : Entity
{
    public Account(Guid publicId, string email, AccountRole role)
    {
        PublicId = publicId;
        Email = email;
        Role = role;
    }
    
    public Guid PublicId { get; private set; }
    public AccountRole Role { get; private set; }
    public string Email { get; private set; }
    
    public void ChangeRole(AccountRole newRole)
    {
        Role = newRole;
    }
}

public enum AccountRole
{
    Worker,
    Admin
}