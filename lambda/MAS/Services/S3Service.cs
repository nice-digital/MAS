using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IS3Service
    {
        Task<PutObjectResponse> WriteToS3(Item item);
    }
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _amazonS3;

        public S3Service(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public async Task<PutObjectResponse> WriteToS3(Item item)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = AppSettings.AWSConfig.BucketName,
                Key = item.Id + ".txt",
                ContentBody = $"Id: {item.Id}\r\nTitle: {item.Title}\r\nSummary: {item.ShortSummary}\r\nSource: {item.Source}"
            };

            var response = await _amazonS3.PutObjectAsync(request);

            return response;
        }
    }
}
