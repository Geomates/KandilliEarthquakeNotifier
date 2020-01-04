using Amazon.SQS;
using Amazon.SQS.Model;
using Common.Models;
using Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface IQueueService
    {
        Task<bool> Enqueue(IEnumerable<TelegramMessage> telegramMessages);
    }

    public class AmazonQueueService : IQueueService
    {
        private const string QUEUE_URL = "SQS_QUEUE_URL";

        private readonly IAmazonSQS _amazonSQS;
        private readonly string _queueUrl;
        public AmazonQueueService(IAmazonSQS amazonSQS, IEnvironmentService environmentService)
        {
            _amazonSQS = amazonSQS;
            _queueUrl = environmentService.GetEnvironmentValue(QUEUE_URL);
        }

        public async Task<bool> Enqueue(IEnumerable<TelegramMessage> telegramMessages)
        {
            var messages = telegramMessages.Select(m => new SendMessageBatchRequestEntry
            {
                Id = Guid.NewGuid().ToString(),
                MessageBody = JsonConvert.SerializeObject(m)
            }).ToList();

            var response = await _amazonSQS.SendMessageBatchAsync(_queueUrl, messages);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
