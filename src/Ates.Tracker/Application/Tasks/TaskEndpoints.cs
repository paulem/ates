using System.Text.Json;
using System.Text.Json.Serialization;
using Ates.SchemaRegistry;
using Ates.Tracker.Application.IntegrationEvents;
using Ates.Tracker.Application.IntegrationEvents.Kafka;
using Ates.Tracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ates.Tracker.Application.Tasks;

public static class TaskEndpoints
{
    public static void AddTaskEndpoints(this IEndpointRouteBuilder app)
    {
        // Get all tasks

        app.MapGet("/tasks", async (TrackerDbContext dbContext) =>
        {
            var tasks = await dbContext.Tasks.Select(t => new
            {
                t.Title,
                t.Description
            }).ToListAsync();

            return Results.Ok(tasks);
        }); //.RequireAuthorization(policyBuilder => policyBuilder.RequireRole("Worker"));

        // Create task

        app.MapPost("/tasks", async (CreateTaskRequest request, TrackerDbContext dbContext, IKafkaProducer producer) =>
        {
            var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.PublicId == request.AssigneeId);

            if (account is null)
                return Results.NotFound("Account not found");

            var task = new Domain.Task(Guid.NewGuid(), request.Title, request.Description, account,
                TaskPrice.Generate(), DateTime.UtcNow);
            
            dbContext.Tasks.Add(task);

            await dbContext.SaveChangesAsync();

            // Produce event

            var @event = new TaskCreatedIntegrationEvent(task.PublicId);
            var kafkaEvent = new KafkaEvent<TaskCreatedIntegrationEvent>(Guid.NewGuid(), 1, DateTime.UtcNow, @event);
            var message = JsonSerializer.Serialize(kafkaEvent,
                new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });

            var result = SchemaValidator.Validate(message, DomainNames.Tasks, kafkaEvent.Name, kafkaEvent.Version);
            
            if (result.IsValid)
            {
                await producer.Produce("tasks-streaming", message, CancellationToken.None);
                await producer.Produce("tasks", message, CancellationToken.None);
            }
            else
            {
                // Log here
            }

            return Results.Created($"/tasks/{task.Id}", new
            {
                PublicId = task.Id,
                AssigneeId = task.Assignee.Id,
                task.Title,
                task.Description,
                task.Status
            });
        });

        // Complete task

        app.MapPost("/tasks/{id:int}/complete", async (int id, TrackerDbContext dbContext, IKafkaProducer producer) =>
        {
            var task = await dbContext.Tasks.FindAsync(id);

            if (task is null)
                return Results.NotFound("Task not found");

            task.Complete();
            await dbContext.SaveChangesAsync();

            // Produce event
            
            var @event = new TaskCompletedIntegrationEvent(task.PublicId);
            var kafkaEvent = new KafkaEvent<TaskCompletedIntegrationEvent>(Guid.NewGuid(), 1, DateTime.UtcNow, @event);
            var message = JsonSerializer.Serialize(kafkaEvent,
            new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
            
            var result = SchemaValidator.Validate(message, DomainNames.Tasks, kafkaEvent.Name, kafkaEvent.Version);
            
            if (result.IsValid)
            {
                await producer.Produce("tasks-streaming", message, CancellationToken.None);
                await producer.Produce("tasks", message, CancellationToken.None);
            }
            else
            {
                // Log here
            }

            return Results.Ok();
        });

        // Shuffle tasks

        app.MapPost("/tasks/shuffle", async (TrackerDbContext dbContext, IKafkaProducer producer) =>
        {
            var openTasks = await dbContext.Tasks
                .Where(t => t.Status == Domain.TaskStatus.Open).ToListAsync();

            var workerAccounts = await dbContext.Accounts
                .Where(p => p.Role == AccountRole.Worker).ToListAsync();

            //var reassignedAccounts = new HashSet<Account>();

            foreach (var openTask in openTasks)
            {
                var rnd = new Random();
                var account = openTask.Assignee;

                while (account == openTask.Assignee)
                    account = workerAccounts[rnd.Next(0, workerAccounts.Count - 1)];

                //reassignedAccounts.Add(account);
                openTask.Reassign(account, DateTime.UtcNow);
            }

            await dbContext.SaveChangesAsync();

            // Produce event

            foreach (var task in openTasks)
            {
                var @event = new TaskReassignedIntegrationEvent(task.PublicId);
                var kafkaEvent = new KafkaEvent<TaskReassignedIntegrationEvent>(Guid.NewGuid(), 1, DateTime.UtcNow, @event);
                var message = JsonSerializer.Serialize(kafkaEvent,
                    new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
            
                var result = SchemaValidator.Validate(message, DomainNames.Tasks, kafkaEvent.Name, kafkaEvent.Version);
            
                if (result.IsValid)
                {
                    await producer.Produce("tasks-streaming", message, CancellationToken.None);
                    await producer.Produce("tasks", message, CancellationToken.None);
                }
                else
                {
                    // Log here
                }
            }

            return Results.Ok();
        });
    }
}