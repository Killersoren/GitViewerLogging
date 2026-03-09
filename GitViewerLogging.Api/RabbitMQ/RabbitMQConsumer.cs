using GitViewerLogging.DataAccess.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace GitViewerLogging.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private readonly IHostEnvironment _environment;
        private readonly string _connectionString;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<RabbitMQConsumer> logger, IHostEnvironment environment)
        {
            _connectionString = settings.Value.ConnectionString;

            _serviceProvider = serviceProvider;
            _logger = logger;
            _environment = environment;

            _factory = new ConnectionFactory();

            if (_environment.IsDevelopment())
            {
                _factory.HostName = "localhost";
            }
            else
            {
                _factory.Uri = new Uri(_connectionString);
            }

        }

        private async Task InitializeAsync()
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: "logs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation("Received message: {Message}", message);
                await CreateLogAsync(message);
            };

            await _channel.BasicConsumeAsync(
                queue: "logs",
                autoAck: true,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task CreateLogAsync(string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LoggingContext>();
            var logEntry = JsonConvert.DeserializeObject<LogEntity>(message);

            if (logEntry != null)
            {
                await context.LogEntities.AddAsync(logEntry);
                await context.SaveChangesAsync();

                await Task.CompletedTask;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
