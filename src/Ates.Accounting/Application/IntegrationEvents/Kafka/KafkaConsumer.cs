using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ates.Accounting.Application.IntegrationEvents.Kafka;

internal class KafkaConsumer : BackgroundService
{
    private readonly IKafkaConsumerMessageHandler _consumerMessageHandler;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly string[] _topics;

    public KafkaConsumer(
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
        IKafkaConsumerMessageHandler consumerMessageHandler, 
        ILogger<KafkaConsumer> logger)
    {
        _consumerMessageHandler = consumerMessageHandler;
        _logger = logger;

        var consumerOptions = kafkaConsumerOptions.Value;
        _topics = consumerOptions.Topics;
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = consumerOptions.BootstrapServers,
            SecurityProtocol = consumerOptions.SecurityProtocol,
            SaslMechanism = consumerOptions.SaslMechanism,
            SaslUsername = consumerOptions.SaslUsername,
            SaslPassword = consumerOptions.SaslPassword,
            GroupId = consumerOptions.GroupId,

            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () => await StartConsumerLoop(_topics, stoppingToken), stoppingToken);
    }

    public override void Dispose()
    {
        // Commit offsets and leave the group cleanly
        _consumer.Close();

        _consumer.Dispose();
        base.Dispose();
    }

    private async Task StartConsumerLoop(string[] topics, CancellationToken cancellationToken)
    {
        _consumer.Subscribe(topics);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                await _consumerMessageHandler.Handle(consumeResult.Topic, consumeResult.Message.Value);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException ex)
            {
                // Consumer errors should generally be ignored (or logged) unless fatal
                _logger.LogError("Consume error: {Reason}", ex.Error.Reason);

                if (ex.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected consumer / message handler error occurred");
                break;
            }
        }
    }
}