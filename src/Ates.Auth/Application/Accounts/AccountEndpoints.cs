using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ates.Auth.Application.IntegrationEvents;
using Ates.Auth.Application.IntegrationEvents.Kafka;
using Ates.Auth.Domain;
using Ates.SchemaRegistry;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ates.Auth.Application.Accounts;

public static class AccountEndpoints
{
    public static void AddAccountEndpoints(this IEndpointRouteBuilder app)
    {
        // Create account
        
        app.MapPost("/account",
            async (RegisterAccountRequest request, AuthDbContext dbContext, IKafkaProducer producer) =>
            {
                if (dbContext.Accounts.Any(x => x.Email == request.Email))
                    return Results.BadRequest("Email is already taken.");

                var account = new Account(Guid.NewGuid(), request.Email, request.Role,
                    BCrypt.Net.BCrypt.HashPassword(request.Password), DateTime.UtcNow);

                // Persist & publish an event

                dbContext.Accounts.Add(account);
                await dbContext.SaveChangesAsync();

                // Produce event

                var @event = new AccountCreatedIntegrationEvent(account.PublicId, account.Email, account.Role);
                var kafkaEvent =
                    new KafkaEvent<AccountCreatedIntegrationEvent>(Guid.NewGuid(), 1, DateTime.UtcNow, @event);
                var message = JsonSerializer.Serialize(kafkaEvent,
                    new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

                var result =
                    SchemaValidator.Validate(message, DomainNames.Accounts, kafkaEvent.Name, kafkaEvent.Version);

                if (result.IsValid)
                {
                    await producer.Produce("accounts-streaming", message, CancellationToken.None);
                    await producer.Produce("accounts", message, CancellationToken.None);
                }
                else
                {
                    // Log here
                }

                //

                return Results.Created($"/accounts/{account.Id}", new
                {
                    account.Id,
                    account.PublicId,
                    account.CreatedAt,
                    account.Email,
                    account.Role
                });
            });

        // Authenticate account

        app.MapPost("/authenticate",
            async (AuthenticateAccountRequest request, IConfiguration config, AuthDbContext dbContext) =>
            {
                var existingAccount = await dbContext.Accounts
                    .FirstOrDefaultAsync(x =>
                        x.Email == request.Email);

                if (existingAccount is null)
                    return Results.NotFound("Account not found.");

                if (!BCrypt.Net.BCrypt.Verify(request.Password, existingAccount.PasswordHash))
                    return Results.NotFound("Password is incorrect.");

                var securityKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException()));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, existingAccount.Email),
                    new Claim(ClaimTypes.Role, existingAccount.Role.ToString())
                };

                var token = new JwtSecurityToken(config["Jwt:Issuer"],
                    config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials);

                return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
            });

        // Change account role

        app.MapPatch("/account", async (ChangeAccountRoleRequest request, AuthDbContext dbContext, IKafkaProducer producer) =>
            {
                var existingAccount = await dbContext.Accounts
                    .FirstOrDefaultAsync(x =>
                        x.Email == request.Email);

                if (existingAccount is null)
                    return Results.NotFound("Account not found.");

                existingAccount.ChangeRole(request.NewRole);
                await dbContext.SaveChangesAsync();

                // Produce event

                var @event = new AccountRoleChangedIntegrationEvent(existingAccount.PublicId, existingAccount.Role);
                var kafkaEvent = new KafkaEvent<AccountRoleChangedIntegrationEvent>(Guid.NewGuid(), version: 1, DateTime.UtcNow, @event);
                var message = JsonSerializer.Serialize(kafkaEvent,
                    new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
                
                var result =
                    SchemaValidator.Validate(message, DomainNames.Accounts, kafkaEvent.Name, kafkaEvent.Version);

                if (result.IsValid)
                {
                    await producer.Produce("accounts-streaming", message, CancellationToken.None);
                    await producer.Produce("accounts", message, CancellationToken.None);
                }
                else
                {
                    // Log here
                }

                return Results.Ok(new
                {
                    existingAccount.Id,
                    existingAccount.PublicId,
                    existingAccount.CreatedAt,
                    existingAccount.Email,
                    existingAccount.Role
                });
            });

        // Get all accounts

        app.MapGet("/accounts", async (AuthDbContext dbContext) =>
        {
            var accounts = await dbContext.Accounts.Select(a => new
            {
                a.Id,
                a.PublicId,
                a.CreatedAt,
                a.Email,
                a.Role
            }).ToListAsync();

            return Results.Ok(accounts);
        });
    }
}