using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NlpPipeline.Engine.Data;

namespace NlpPipeline.Engine
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IModel? _channel;
        private const string QueueName = "nlp_analysis_queue";

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            InitRabbitMq();
        }

        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                using var jsonDoc = JsonDocument.Parse(message);
                var root = jsonDoc.RootElement;
                Guid requestId = root.GetProperty("RequestId").GetGuid();
                string text = root.GetProperty("Text").GetString() ?? "";

                _logger.LogInformation($"[İŞLENİYOR] Request Id: {requestId} için NLP analizi başladı.");

                // NLP Morfolojik Analiz Simülasyonu (Algoritma Katmanı)
                var analysisResult = AnalyzeTextMorphology(text);

                // Veritabanını Güncelleme
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var request = await dbContext.AnalysisRequests.FindAsync(requestId);
                    if (request != null)
                    {
                        request.Status = "Completed";
                        request.AnalysisResult = JsonSerializer.Serialize(analysisResult);
                        request.CompletedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                    }
                }

                _logger.LogInformation($"[TAMAMLANDI] Request Id: {requestId} veritabanına yazıldı.");
                _channel!.BasicAck(ea.DeliveryTag, false);
            };

            _channel!.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        // Dilbilimsel Kurallara Göre Gelişmiş Morfolojik Analiz Motoru
        private object AnalyzeTextMorphology(string text)
        {
            var words = text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var analyzedWords = new List<object>();

            foreach (var word in words)
            {
                string cleanWord = word.ToLower();
                string root = cleanWord;
                string suffix = "Yok";

                // Örnek kurallı Türkçe kök/ek ayrım motoru simülasyonu
                if (cleanWord.EndsWith("lar") || cleanWord.EndsWith("ler"))
                {
                    root = cleanWord[..^3];
                    suffix = cleanWord[^3..] + " (Çoğul Eki)";
                }
                else if (cleanWord.EndsWith("da") || cleanWord.EndsWith("de") || cleanWord.EndsWith("ta") || cleanWord.EndsWith("te"))
                {
                    root = cleanWord[..^2];
                    suffix = cleanWord[^2..] + " (Bulunma Durum Eki)";
                }
                else if (cleanWord.EndsWith("ın") || cleanWord.EndsWith("in") || cleanWord.EndsWith("un") || cleanWord.EndsWith("ün"))
                {
                    root = cleanWord[..^2];
                    suffix = cleanWord[^2..] + " (İlgi/İyelik Eki)";
                }

                analyzedWords.Add(new { Kelime = word, Kok = root, Ek = suffix });
            }

            return new { ToplamKelime = words.Length, KelimeAnalizleri = analyzedWords };
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}