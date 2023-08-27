using System.Text.Json;
using System.Text.Json.Serialization;
using Ates.Accounting.Application.IntegrationEvents.Kafka;
using Ates.SchemaRegistry;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Ates.Accounting.Application.IntegrationEvents.EventHandlers;

public class TaskCreatedIntegrationEventHandler : INotificationHandler<TaskCreatedIntegrationEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TaskCreatedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task Handle(TaskCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();
        
        var task = await dbContext.Tasks
            .FirstOrDefaultAsync(t => t.PublicId == notification.TaskId, cancellationToken);

        if (task is null)
        {
            task = new Domain.Task(
                notification.TaskId,
                notification.Title,
                notification.Fee,
                notification.Reward);
            
            dbContext.Tasks.Add(task);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        //

        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.PublicId == notification.AssigneeId, cancellationToken);

        if (account is null)
            throw new InvalidOperationException("Account not found.");

        var transaction = account.ChargeAssignmentFee(task.Fee, $"Assignment fee for {task.Id} is charged");
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