using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Common.Models;
using Common.Services;
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

            foreach (var record in sqsEvent.Records)
            {
                LambdaLogger.Log($"Message ID: {record.MessageId}");
                var telegramMessage = JsonConvert.DeserializeObject<TelegramMessage>(record.Body);
                await telegramService.SendMessage(telegramMessage);
            }

            LambdaLogger.Log($"Processed {sqsEvent.Records.Count} records.\n");
        }

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddTransient<ITelegramService, TelegramService>();
        }
    }
}
