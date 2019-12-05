using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using System;
using System.Text;
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
                ContentBody = CreateContentBody(item)
            };

            var response = await _amazonS3.PutObjectAsync(request);

            return response;
        }

        private string CreateContentBody(Item item)
        {
            var contentBody = new StringBuilder();
            contentBody.Append("Title: ");
            contentBody.Append(item.Title);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Short Summary: ");
            contentBody.Append(item.ShortSummary);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Source: ");
            contentBody.Append(item.Source.Title);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Evidence Type: ");
            contentBody.Append(item.EvidenceType);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("UKMI Comment: ");
            contentBody.Append(item.UKMiComment);
            contentBody.Append(Environment.NewLine);

            if (item.ResourceLinks != null)
            {
                contentBody.Append("Resource Links: ");
                contentBody.Append(item.ResourceLinks);
            }

            return contentBody.ToString();
        }
    }
}
