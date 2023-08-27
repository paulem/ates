using System.Text;
using System.Text.Json.Serialization;
using Ates.Tracker;
using Ates.Tracker.Application.IntegrationEvents.Kafka;
using Ates.Tracker.Application.Tasks;
using Ates.Tracker.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;
using TaskStatus = Ates.Tracker.Domain.TaskStatus;

var builder = WebApplication.CreateBuilder(args);

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Default"));
dataSourceBuilder.MapEnum<AccountRole>();
dataSourceBuilder.MapEnum<TaskStatus>();

await using var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<TrackerDbContext>(options =>
    {
        options.UseNpgsql(dataSource);
        options.UseSnakeCaseNamingConvention();
    }
);

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerJwtOptions>();

builder.Services.Configure<KafkaProducerOptions>(builder.Configuration.GetSection("Kafka:Producer"));
builder.Services.Configure<KafkaConsumerOptions>(builder.Configuration.GetSection("Kafka:Consumer"));

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddTransient<IKafkaConsumerMessageHandler, KafkaConsumerMessageHandler>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TrackerDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

app.AddTaskEndpoints();

//

app.Run();