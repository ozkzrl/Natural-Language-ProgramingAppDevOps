using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace NlpPipeline.Api.Services
{
    public class RabbitMqService
    {
        private readonly IConfiguration _configuration;
        private const string QueueName = "nlp_analysis_queue";

        public RabbitMqService(IConfiguration configuration) => _configuration = configuration;

        public void PublishAnalysisTask(Guid requestId, string text)
        {
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:Host"] ?? "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messagePayload = new { RequestId = requestId, Text = text };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messagePayload));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: properties, body: body);
        }
    }
}