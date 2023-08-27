using Ates.Tracker.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Ates.Tracker.Application.IntegrationEvents.EventHandlers;

public class AccountCreatedIntegrationEventHandler : INotificationHandler<AccountCreatedIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AccountCreatedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task Handle(AccountCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TrackerDbContext>();
        
        var existingAccount = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.PublicId == notification.PublicId, cancellationToken);

        if (existingAccount is null)
        {
            dbContext.Accounts.Add(new Account(notification.PublicId, notification.Email, notification.Role));
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}