using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface IQueueService
    {
        Task<bool> Enqueue(IEnumerable<TelegramMessage> telegramMessages);
    }

    public class AmazonQueueService : IQueueService
    {
        private const string QueueUrl = "SQS_QUEUE_URL";

        private readonly IAmazonSQS _amazonSqs;
        private readonly string _queueUrl;
        public AmazonQueueService(IAmazonSQS amazonSqs, IEnvironmentService environmentService)
        {
            _amazonSqs = amazonSqs;
            _queueUrl = environmentService.GetEnvironmentValue(QueueUrl);
        }

        public async Task<bool> Enqueue(IEnumerable<TelegramMessage> telegramMessages)
        {
            var messages = telegramMessages.Select(m => new SendMessageBatchRequestEntry
            {
                Id = Guid.NewGuid().ToString(),
                MessageBody = JsonSerializer.Serialize(m)
            }).ToList();

            var response = await _amazonSqs.SendMessageBatchAsync(_queueUrl, messages);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
