using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface ISubscribtionService
    {
        Task<IEnumerable<int>> GetByMagnitudeAsync(double magnitude);
    }

    public class AmazonDynamoDBService : ISubscribtionService
    {
        private const string TABLE_NAME = "DYNAMODB_TABLE_NAME";

        private readonly IAmazonDynamoDB _amazonDynamoDB;
        private readonly string _tableName;

        public AmazonDynamoDBService(IAmazonDynamoDB amazonDynamoDB, IEnvironmentService environmentService)
        {
            _amazonDynamoDB = amazonDynamoDB;
            _tableName = environmentService.GetEnvironmentValue(TABLE_NAME);
        }

        public async Task<IEnumerable<int>> GetByMagnitudeAsync(double magnitude)
        {
            var result = new List<int>();

            Dictionary<string, Condition> conditions = new Dictionary<string, Condition>
            {
                {
                    "Magnitude",
                    new Condition
                    {
                        ComparisonOperator = ComparisonOperator.LE,
                        AttributeValueList = new List<AttributeValue>{ new AttributeValue { N = magnitude.ToString() } }
                    }
                }
            };

            ScanRequest request = new ScanRequest
            {
                TableName = _tableName,
                ExclusiveStartKey = null,
                ScanFilter = conditions
            };

            ScanResponse scanResponse = await _amazonDynamoDB.ScanAsync(request);

            foreach (var item in scanResponse.Items)
            {
                if (
                    !item.ContainsKey("Magnitude") ||
                    (item.ContainsKey("Magnitude") && double.TryParse(item["Magnitude"].N, out double minMagnitude) && magnitude >= minMagnitude))
                {
                    if (item.ContainsKey("chatid") && int.TryParse(item["chatid"].S, out int chatId))
                    {
                        result.Add(chatId);
                    }
                }
            }

            return result;
        }
    }
}
