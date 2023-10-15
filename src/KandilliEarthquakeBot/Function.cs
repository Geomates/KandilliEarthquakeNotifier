using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common.Exceptions;
using Common.Services;
using KandilliEarthquakeBot.Enums;
using KandilliEarthquakeBot.Helpers;
using KandilliEarthquakeBot.Models;
using KandilliEarthquakeBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace KandilliEarthquakeBot
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(request.Body);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            WebhookMessage webhookMessage = null;

            try
            {
                webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(request.Body);
            }
            catch(Exception ex)
            {
                LambdaLogger.Log("Error: " + ex.Message);
            }
            
            if (webhookMessage == null)
            {
                return response;
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var botService = serviceProvider.GetService<IBotService>();

            if (webhookMessage.Message?.Text != null && CommandHelper.TryParseCommand(webhookMessage.Message.Text, out Command command))
            {
                try
                {
                    switch (command)
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
                        case Command.RemoveLocation:
                            await botService.RemoveLocationAsync(webhookMessage.Message.Chat.Id);
                            break;
                        case Command.About:
                            await botService.About(webhookMessage.Message.Chat.Id);
                            break;
                    }
                }
                catch (TelegramApiException exception) when (exception.Response.ErrorCode == 403) //bot blocked, delete subscription
                {
                    await botService.RemoveSubscriptionAsync(webhookMessage.Message.Chat.Id);
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
                            await botService.SetMagnitudeAsync(webhookMessage.CallbackQuery.Message.MessageId, webhookMessage.CallbackQuery.Id, webhookMessage.CallbackQuery.Message.Chat.Id, magnitude);
                        }                        
                        break;
                }
            }            

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
