using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;
using KandilliEarthquakeBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KandilliEarthquakeBot.Services
{
    public interface ISubscribtionStore
    {
        Task<bool> UpdateAsync(SubscriptionUpdateRequest updateRequest);
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

        public async Task<bool> UpdateAsync(SubscriptionUpdateRequest updateRequest)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"chatid", new AttributeValue {S = updateRequest.SubscriptionId.ToString()}}
            };

            foreach (var updatedProperty in updateRequest.UpdatedProperties)
            {
                switch (Type.GetTypeCode(updatedProperty.Type))
                {
                    case TypeCode.String:
                        item.Add(updatedProperty.Name, new AttributeValue { S = updatedProperty.Value.ToString() });
                        break;
                    case TypeCode.UInt64:
                    case TypeCode.Double:
                        item.Add(updatedProperty.Name, new AttributeValue { N = updatedProperty.Value.ToString() });
                        break;
                }

            }

            var putItemRequest = new PutItemRequest
            {
                TableName = _tableName,
                Item = item
            };

            var response = await _amazonDynamoDB.PutItemAsync(putItemRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
