using System;
using System.Linq;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.CloudWatchEvents;
using Microsoft.Extensions.DependencyInjection;
using KandilliEarthquakeNotifier.Services;
using Amazon.S3;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace KandilliEarthquakeNotifier
{
    public class Function
    {
        public async Task FunctionHandler(CloudWatchEvent<object> cloudWatchLogsEvent, ILambdaContext context)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();            

            var bookmarkService = serviceProvider.GetService<IBookmarkService>();
            var kandilliService = serviceProvider.GetService<IKandilliService>();
            var telegramService = serviceProvider.GetService<ITelegramService>();

            var lastFetch = await bookmarkService.GetBookmark("last_fetch_date");

            DateTime.TryParse(lastFetch, out DateTime lastFetchDate);           

            var earthquakes = await kandilliService.GetEarthquakes();

            var newEarthquakes = earthquakes
                .Where(q => q.Date > lastFetchDate)
                .OrderByDescending(q => q.Date)
                .Take(10)
                .OrderBy(q => q.Date);

            var lastNotifiedEarthQuake = lastFetchDate;

            foreach(var newEarthQuake in newEarthquakes)
            {
                var result = await telegramService.SendMessage(newEarthQuake.ToTelegramMessage(), newEarthQuake.Magnitude < 4);
                if (!result)
                {
                    break;
                }
                lastNotifiedEarthQuake = newEarthQuake.Date;
            }

            await bookmarkService.SetBookmark("last_fetch_date", lastNotifiedEarthQuake.ToString());
        }

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IBookmarkService, BookmarkService>();
            serviceCollection.AddTransient<IKeyValueStore, AmazonS3Service>();
            serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddTransient<IKandilliService, KandilliService>();
            serviceCollection.AddTransient<ITelegramService, TelegramService>();
            serviceCollection.AddAWSService<IAmazonS3>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
        }
    }
}
