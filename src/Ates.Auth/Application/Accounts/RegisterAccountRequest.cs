using System.ComponentModel.DataAnnotations;
using Ates.Auth.Domain;

namespace Ates.Auth.Application.Accounts;

public class RegisterAccountRequest
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
    
    [Required]
    public AccountRole Role { get; set; }
}