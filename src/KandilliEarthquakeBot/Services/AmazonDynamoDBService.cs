using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;
using KandilliEarthquakeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KandilliEarthquakeBot.Services
{
    public interface ISubscribtionStore
    {
        Task<bool> UpdateAsync(SubscriptionUpdateRequest updateRequest);
        Task<bool> RemoveAsync(SubscriptionUpdateRequest updateRequest);
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
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> { { "chatid", new AttributeValue { S = updateRequest.SubscriptionId.ToString() } } }
            };

            if (updateRequest.UpdatedProperties.Count() > 0)
            {
                var expressionAttributeValues = new Dictionary<string, AttributeValue>();
                var expressionAttributeNames = new Dictionary<string, string>();

                List<string> updateExpressions = new List<string>();
                int i = 0;
                foreach (var updatedProperty in updateRequest.UpdatedProperties)
                {
                    updateExpressions.Add($"#P{i}=:P{i}val");
                    expressionAttributeNames.Add($"#P{i}", updatedProperty.Name);
                    switch (Type.GetTypeCode(updatedProperty.Type))
                    {
                        case TypeCode.String:
                            expressionAttributeValues.Add($":P{i}val", new AttributeValue { S = updatedProperty.Value.ToString() });
                            break;
                        case TypeCode.UInt64:
                        case TypeCode.Double:
                            expressionAttributeValues.Add($":P{i}val", new AttributeValue { N = updatedProperty.Value.ToString() });
                            break;
                    }
                    i++;
                }
                updateItemRequest.UpdateExpression = "SET " + string.Join(',', updateExpressions);
                updateItemRequest.ExpressionAttributeNames = expressionAttributeNames;
                updateItemRequest.ExpressionAttributeValues = expressionAttributeValues;
            }

            var response = await _amazonDynamoDB.UpdateItemAsync(updateItemRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> RemoveAsync(SubscriptionUpdateRequest updateRequest)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> { { "chatid", new AttributeValue { S = updateRequest.SubscriptionId.ToString() } } }
            };
            var response = await _amazonDynamoDB.DeleteItemAsync(deleteItemRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
