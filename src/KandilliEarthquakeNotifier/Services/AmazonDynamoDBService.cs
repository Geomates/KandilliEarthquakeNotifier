using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KandilliEarthquakeNotifier.Services
{
    public interface ISubscribtionStore
    {
        Task<bool> RemoveAsync(int subscriptionId);
    }

    public class AmazonDynamoDBService : ISubscribtionStore
    {
        private const string TABLE_NAME = "DYNAMODB_TABLE_NAME";

        private readonly IAmazonDynamoDB _amazonDynamoDB;
        private readonly string _tableName;

        public AmazonDynamoDBService(IAmazonDynamoDB amazonDynamoDB, IEnvironmentService environmentService)
        {
            _amazonDynamoDB = amazonDynamoDB;
            _tableName = environmentService.GetEnvironmentValue(TABLE_NAME);
        }        

        public async Task<bool> RemoveAsync(int subscriptionId)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> { { "chatid", new AttributeValue { S = subscriptionId.ToString() } } }
            };
            var response = await _amazonDynamoDB.DeleteItemAsync(deleteItemRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
