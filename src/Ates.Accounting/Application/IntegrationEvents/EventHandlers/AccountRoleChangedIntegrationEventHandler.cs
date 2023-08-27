using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Ates.Accounting.Application.IntegrationEvents.EventHandlers;

public class AccountRoleChangedIntegrationEventHandler : INotificationHandler<AccountCreatedIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AccountRoleChangedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task Handle(AccountCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        
        var existingAccount = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.PublicId == notification.PublicId, cancellationToken);

        if (existingAccount is not null && existingAccount.Role != notification.Role)
        {
            existingAccount.ChangeRole(notification.Role);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}