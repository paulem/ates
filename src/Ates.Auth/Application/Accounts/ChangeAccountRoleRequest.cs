using System.ComponentModel.DataAnnotations;
using Ates.Auth.Domain;

namespace Ates.Auth.Application.Accounts;

public class ChangeAccountRoleRequest
{
    [Required]
    public required string Email { get; set; }
    
    [Required]
    public required AccountRole NewRole { get; set; }
}