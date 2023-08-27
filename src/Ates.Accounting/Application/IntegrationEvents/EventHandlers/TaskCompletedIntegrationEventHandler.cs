using System.Text.Json;
using System.Text.Json.Serialization;
using Ates.Accounting.Application.IntegrationEvents.Kafka;
using Ates.SchemaRegistry;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Ates.Accounting.Application.IntegrationEvents.EventHandlers;

public class TaskCompletedIntegrationEventHandler : INotificationHandler<TaskCompletedIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TaskCompletedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task Handle(TaskCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
        
        var existingTask = await dbContext.Tasks
            .FirstOrDefaultAsync(a => a.PublicId == notification.TaskId, cancellationToken);

        if (existingTask is null)
            throw new InvalidOperationException("Task not found.");
        
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.PublicId == notification.AssigneeId,
            cancellationToken);
        
        if (account is null)
            throw new InvalidOperationException("Account not found.");
        
        var transaction = account.DepositCompletionReward(existingTask.Reward, $"Completion reward for {existingTask.PublicId} is deposited.");
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // Produce event
            
        var @event = new TransactionCommittedIntegrationEvent(transaction.PublicId, transaction.Debit, transaction.Credit, transaction.Message);
        var kafkaEvent = new KafkaEvent<TransactionCommittedIntegrationEvent>(Guid.NewGuid(), 1, DateTime.UtcNow, @event);
        var message = JsonSerializer.Serialize(kafkaEvent,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
            
        var result = SchemaValidator.Validate(message, DomainNames.Accounting, kafkaEvent.Name, kafkaEvent.Version);

        if (result.IsValid)
            await producer.Produce("accounting-lifetime", message, CancellationToken.None);
        else
        {
            // Log here
        }
    }
}