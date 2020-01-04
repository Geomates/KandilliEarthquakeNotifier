using Amazon.S3;
using Amazon.S3.Model;
using Common.Services;
using System.IO;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface IKeyValueStore
    {
        Task<string> Read(string key);
        Task<bool> Write(string key, string value);
    }

    public class AmazonS3Service : IKeyValueStore
    {
        private const string S3_BUCKET = "BOOKMARK_STORE_BUCKET";
        private const string S3_PREFIX = "BOOKMARK_STORE_PREFIX";

        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;
        private readonly string _s3Prefix;

        public AmazonS3Service(IAmazonS3 amazonS3, IEnvironmentService environmentService)
        {
            _amazonS3 = amazonS3;

            _bucketName = environmentService.GetEnvironmentValue(S3_BUCKET);
            _s3Prefix = environmentService.GetEnvironmentValue(S3_PREFIX);
        }

        public async Task<string> Read(string key)
        {
            using (GetObjectResponse response = await _amazonS3.GetObjectAsync(_bucketName, $"{_s3Prefix}/{key}"))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<bool> Write(string key, string value)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{_s3Prefix}/{key}",
                ContentBody = value
            };

            var putObjectResponse = await _amazonS3.PutObjectAsync(putObjectRequest);

            return putObjectResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
