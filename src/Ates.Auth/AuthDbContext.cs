using Ates.Auth.Domain;
using Ates.Auth.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ates.Auth;

public class AuthDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;
    
    public AuthDbContext (DbContextOptions<AuthDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }
    
    //

    public DbSet<Account> Accounts => Set<Account>();
    
    //

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AccountRole>();
        
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Ignore(e => e.DomainEvents);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        var domainEntities = ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any()).ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);
        
        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        return await base.SaveChangesAsync(cancellationToken);
    }
}