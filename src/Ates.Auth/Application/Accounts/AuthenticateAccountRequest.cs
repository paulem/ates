using System.ComponentModel.DataAnnotations;

namespace Ates.Auth.Application.Accounts;

public class AuthenticateAccountRequest
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}