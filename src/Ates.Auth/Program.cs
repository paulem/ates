using System.Text.Json.Serialization;
using Ates.Auth;
using Ates.Auth.Application.Accounts;
using Ates.Auth.Application.IntegrationEvents.Kafka;
using Ates.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Default"));
dataSourceBuilder.MapEnum<AccountRole>();

await using var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AuthDbContext>(options =>
    {
        options.UseNpgsql(dataSource);
        options.UseSnakeCaseNamingConvention();
    }
);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaProducerOptions>(builder.Configuration.GetSection("Kafka:Producer"));
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AuthDbContext>();
    
    //context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

//

app.AddAccountEndpoints();

//

app.Run();