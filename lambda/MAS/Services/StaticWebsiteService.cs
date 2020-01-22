using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IStaticWebsiteService
    {
        /// <summary>
        /// Asynchronous writes a file to the statice website
        /// </summary>
        /// <param name="filePath">The path of the file, relative to the root e.g. "sitemap.xml"</param>
        /// <param name="contentBody">The body of the file</param>
        /// <returns>The http status code of the request to write the file</returns>
        Task<HttpStatusCode> WriteFileAsync(string filePath, string contentBody);

        Task<HttpStatusCode> WriteFileAsync(string filePath, Stream inputStream);
    }
    public class S3StaticWebsiteService : IStaticWebsiteService
    {
        #region Constructor

        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3StaticWebsiteService> _logger;
        private readonly AWSConfig _awsConfig;

        public S3StaticWebsiteService(IAmazonS3 amazonS3, ILogger<S3StaticWebsiteService> logger, AWSConfig awsConfig)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
            _awsConfig = awsConfig;
        }

        #endregion

        public async Task<HttpStatusCode> WriteFileAsync(string filePath, string contentBody)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _awsConfig.BucketName,
                Key = filePath,
                ContentBody = contentBody
            };

            try
            {
                var response = await _amazonS3.PutObjectAsync(request);
                return response.HttpStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to write ${filePath} to S3: {e.Message}");
                throw e;
            }
        }
        public async Task<HttpStatusCode> WriteFileAsync(string filePath, Stream inputStream)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _awsConfig.BucketName,
                Key = filePath,
                InputStream = inputStream
            };

            try
            {
                var response = await _amazonS3.PutObjectAsync(request);
                return response.HttpStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to write ${filePath} to S3: {e.Message}");
                throw e;
            }
        }
    }
}
