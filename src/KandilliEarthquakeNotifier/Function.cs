using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common.Exceptions;
using Common.Models;
using Common.Services;
using KandilliEarthquakeNotifier.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace KandilliEarthquakeNotifier
{
    public class Function
    {
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            LambdaLogger.Log("Start\n");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var telegramService = serviceProvider.GetService<ITelegramService>();
            var subscriptionService = serviceProvider.GetService<ISubscribtionStore>();

            foreach (var record in sqsEvent.Records)
            {
                LambdaLogger.Log($"Message ID: {record.MessageId}");
                var telegramMessage = JsonConvert.DeserializeObject<TelegramMessage>(record.Body);
                try
                {
                    await telegramService.SendMessage(telegramMessage);
                }
                catch(TelegramApiException exception) when (exception.Response.ErrorCode == 403) //bot blocked, delete subscription
                {
                    if (int.TryParse(telegramMessage.ChatId, out int chatId))
                    {
                        await subscriptionService.RemoveAsync(chatId);
                    }
                }
            }

            LambdaLogger.Log($"Processed {sqsEvent.Records.Count} records.\n");
        }

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddTransient<ITelegramService, TelegramService>();
            serviceCollection.AddTransient<ISubscribtionStore, AmazonDynamoDBService>();
            serviceCollection.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
        }
    }
}
