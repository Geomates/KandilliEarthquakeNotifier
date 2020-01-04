using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Helpers;
using Common.Services;
using Google.Common.Geometry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface ISubscribtionService
    {
        Task<IEnumerable<int>> GetAsync(double magnitude, double latitude, double longitude, int searchRadius);
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

        public async Task<IEnumerable<int>> GetAsync(double magnitude, double latitude, double longitude, int searchRadius)
        {
            var result = new List<int>();

            var searchRectangle = S2Manager.GetBoundingLatLngRect(latitude, longitude, searchRadius);

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

            Dictionary<string, AttributeValue> lastEvaluatedKey = null;

            do 
            {
                ScanRequest request = new ScanRequest
                {
                    TableName = _tableName,
                    ScanFilter = conditions
                };

                if (lastEvaluatedKey != null && lastEvaluatedKey.ContainsKey("chatid"))
                {
                    request.ExclusiveStartKey["chatid"] = lastEvaluatedKey["chatid"];
                }

                ScanResponse scanResponse = await _amazonDynamoDB.ScanAsync(request);

                foreach (var item in scanResponse.Items)
                {
                    if (item.ContainsKey("chatid") && int.TryParse(item["chatid"].S, out int chatId))
                    {
                        if (item.ContainsKey("LocationHash"))
                        {
                            if (ulong.TryParse(item["LocationHash"].N, out ulong locationHash) && searchRectangle.Intersects(new S2Cell(new S2CellId(locationHash))))
                            {
                                result.Add(chatId);
                            }
                        }
                        else
                        {
                            result.Add(chatId);
                        }
                    }
                }
                lastEvaluatedKey = scanResponse.LastEvaluatedKey;
            } while (lastEvaluatedKey != null && lastEvaluatedKey.Count > 0);

            return result;
        }
    }
}
