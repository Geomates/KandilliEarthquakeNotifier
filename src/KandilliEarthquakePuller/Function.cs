using Amazon.DynamoDBv2;
using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Amazon.SQS;
using Common.Models;
using Common.Services;
using KandilliEarthquakePuller.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace KandilliEarthquakePuller
{
    public class Function
    {
        private const string TELEGRAM_CHANNEL_NAME = "TELEGRAM_CHANNEL_NAME";
        public async Task FunctionHandler(CloudWatchEvent<object> cloudWatchLogsEvent, ILambdaContext context)
        {
            LambdaLogger.Log("Start\n");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();            

            var bookmarkService = serviceProvider.GetService<IBookmarkService>();
            var kandilliService = serviceProvider.GetService<IKandilliService>();
            var environmentService = serviceProvider.GetService<IEnvironmentService>();
            var queueService = serviceProvider.GetService<IQueueService>();
            var subscriptionService = serviceProvider.GetService<ISubscribtionService>();

            var lastFetch = await bookmarkService.GetBookmark("last_fetch_date");

            var channelName = environmentService.GetEnvironmentValue(TELEGRAM_CHANNEL_NAME);

            DateTime.TryParse(lastFetch, out DateTime lastFetchDate);           

            var earthquakes = await kandilliService.GetEarthquakes();

            var newEarthquakes = earthquakes
                .Where(q => q.Date > lastFetchDate)
                .OrderByDescending(q => q.Date)
                .Take(10)
                .OrderBy(q => q.Date);

            if (newEarthquakes.Count() == 0)
            {
                LambdaLogger.Log("No new earthquakes\nFinish\n");
                return;
            }

            LambdaLogger.Log(newEarthquakes.Count() + " new earthquakes\n");

            var lastNotifiedEarthQuake = lastFetchDate;
            int searchRadius = 100000; //in meters, 100km

            foreach (var newEarthQuake in newEarthquakes)
            {                
                var subscribers = await subscriptionService.GetAsync(newEarthQuake.Magnitude, newEarthQuake.Latitude, newEarthQuake.Longitude, searchRadius);
                int i = 0;
                int chunkSize = 10;
                int[][] chunks = subscribers.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();

                //Send to subscribers
                foreach(var chunk in chunks)
                {
                    await queueService.Enqueue(chunk.Select(c => new TelegramMessage
                    {
                        ChatId = c.ToString(),
                        ParseMode = "markdown",
                        DisableWebPagePreview = true,
                        Text = newEarthQuake.ToTelegramMessage()
                    }));
                }

                //Send to channel
                await queueService.Enqueue(new List<TelegramMessage>
                {
                    new TelegramMessage
                    {
                        ChatId = $"@{channelName}",
                        DisableNotification = newEarthQuake.Magnitude < 4,
                        ParseMode = "markdown",
                        DisableWebPagePreview = true,
                        Text = newEarthQuake.ToTelegramMessage()
                    }
                });
                
                lastNotifiedEarthQuake = newEarthQuake.Date;
            }

            await bookmarkService.SetBookmark("last_fetch_date", lastNotifiedEarthQuake.ToString());

            LambdaLogger.Log("Finish\n");
        }

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IBookmarkService, BookmarkService>();
            serviceCollection.AddTransient<IKeyValueStore, AmazonS3Service>();
            serviceCollection.AddTransient<IQueueService, AmazonQueueService>();
            serviceCollection.AddTransient<ISubscribtionService, AmazonDynamoDBService>();
            serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddTransient<IKandilliService, KandilliService>();
            serviceCollection.AddAWSService<IAmazonS3>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            serviceCollection.AddAWSService<IAmazonSQS>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            serviceCollection.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
        }
    }
}
