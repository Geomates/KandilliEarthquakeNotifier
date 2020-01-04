using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Services;
using KandilliEarthquakeBot.Enums;
using KandilliEarthquakeBot.Helpers;
using KandilliEarthquakeBot.Models;
using KandilliEarthquakeBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace KandilliEarthquakeBot
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(request.Body);

            WebhookMessage webhookMessage = JsonConvert.DeserializeObject<WebhookMessage>(request.Body);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var botService = serviceProvider.GetService<IBotService>();

            if (webhookMessage.Message?.Text != null && CommandHelper.TryParseCommand(webhookMessage.Message.Text, out Command command))
            {
                switch(command)
                {
                    case Command.Start:
                        await botService.StartSubscriptionAsync(webhookMessage.Message.Chat.Id);
                        break;
                    case Command.Stop:
                        await botService.RemoveSubscriptionAsync(webhookMessage.Message.Chat.Id);
                        break;
                    case Command.Magnitude:
                        await botService.AskMagnitudeAsync(webhookMessage.Message.Chat.Id);
                        break;
                    case Command.Location:
                        await botService.AskLocationAsync(webhookMessage.Message.Chat.Id);
                        break;
                }
            }

            if (webhookMessage.Message?.Location != null)
            {
                await botService.SetLocationAsync(webhookMessage.Message.Chat.Id, webhookMessage.Message.Location);
            }

            if (webhookMessage.CallbackQuery != null)
            {

                switch (webhookMessage.CallbackQuery.Message.Text)
                {
                    case BotDialog.ASK_MAGNITUDE:
                        if (double.TryParse(webhookMessage.CallbackQuery.Data, out double magnitude))
                        {
                            await botService.SetMagnitudeAsync(webhookMessage.CallbackQuery.Message.MessageId, webhookMessage.CallbackQuery.Message.Chat.Id, magnitude);
                        }                        
                        break;
                }
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IBotService, BotService>();
            serviceCollection.AddTransient<ISubscribtionStore, AmazonDynamoDBService>();
            serviceCollection.AddTransient<ISubscriptionUpdateRequestBuilder, SubscriptionUpdateRequestBuilder>();
            serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddTransient<ITelegramService, TelegramService>();
            serviceCollection.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
        }
    }
}
