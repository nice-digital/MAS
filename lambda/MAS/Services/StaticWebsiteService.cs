using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public class StaticContentRequest
    {
        public string ContentBody { get; set; }

        public Stream ContentStream { get; set; }

        public string FilePath { get; set; }
    }

    public interface IStaticWebsiteService
    {
        /// <summary>
        /// Asynchronous writes a file to the statice website
        /// </summary>
        /// <param name="filePath">The path of the file, relative to the root e.g. "sitemap.xml"</param>
        /// <param name="contentBody">The body of the file</param>
        /// <returns>The http status code of the request to write the file</returns>
        //Task<HttpStatusCode> WriteFileAsync(string filePath, string contentBody);

        //Task<HttpStatusCode> WriteFileAsync(string filePath, Stream inputStream);

        //Task<HttpStatusCode> InvalidateCacheAsync(string paths);

        //Task<HttpStatusCode> WriteContentAsync(string filePath, dynamic contentOrStream);

        Task<HttpStatusCode> WriteFilesAsync(params StaticContentRequest[] requests);
    }
    public class S3StaticWebsiteService : IStaticWebsiteService
    {
        #region Constructor

        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3StaticWebsiteService> _logger;
        private readonly AWSConfig _awsConfig;
        private readonly IAmazonCloudFront _cloudFrontService;
        private readonly EnvironmentConfig _environmentConfig;
        private readonly CloudFrontConfig _cloudFrontConfig;

        public S3StaticWebsiteService(IAmazonS3 amazonS3, 
            IAmazonCloudFront cloudFrontService, 
            ILogger<S3StaticWebsiteService> logger,
            AWSConfig awsConfig, 
            EnvironmentConfig environmentConfig,
            CloudFrontConfig cloudFrontConfig)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
            _awsConfig = awsConfig;
            _cloudFrontService = cloudFrontService;
            _environmentConfig = environmentConfig;
            _cloudFrontConfig = cloudFrontConfig;
        }

        #endregion


        public async Task<HttpStatusCode> WriteFilesAsync(params StaticContentRequest[] requests)
        {
            _logger.LogDebug($"Hit WriteFilesAsync");
            HttpStatusCode responseCode = HttpStatusCode.OK;
            var taskDict = new Dictionary<StaticContentRequest, Task<PutObjectResponse>>();
            foreach(var req in requests)
                taskDict.Add(req, WriteFileAsync(req));

            foreach(var task in taskDict)
            {
                responseCode = (await task.Value).HttpStatusCode;
                if (responseCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing {task.Key} resulted in a status code of {responseCode}");
                    return responseCode;
                }
            }

            if (_environmentConfig.Name != "local")
                responseCode = InvalidateCacheAsync(taskDict.Select(x => x.Key.FilePath).ToList());

            _logger.LogDebug($"Completed WriteFilesAsync");
            return responseCode;

        }
        
        private Task<PutObjectResponse> WriteFileAsync(StaticContentRequest item)
        {
            _logger.LogDebug($"Hit WriteFileAsync");
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _awsConfig.BucketName,
                Key = item.FilePath
            };

            if (string.IsNullOrEmpty(request.ContentBody))
                request.InputStream = item.ContentStream;
            else
                request.ContentBody = item.ContentBody;

            _logger.LogDebug($"Finished WriteFileAsync");
            return _amazonS3.PutObjectAsync(request);
        }

        private HttpStatusCode InvalidateCacheAsync(List<string> paths)
        {
            _logger.LogDebug($"Clear cache hit");
            _logger.LogDebug($"Cache clearing  dist id is: {0} ", _cloudFrontConfig.DistributionID);
            

            var invalidationBatch = new InvalidationBatch() { Paths = new Paths() { Items = paths } };

            var req = new CreateInvalidationRequest(_cloudFrontConfig.DistributionID, invalidationBatch);
            var invalidateCacheResponseCode = _cloudFrontService.CreateInvalidationAsync(req).Result.HttpStatusCode;

            if (invalidateCacheResponseCode != HttpStatusCode.OK)
                _logger.LogError($"Cache invalidation failed and resulted in a status code of {invalidateCacheResponseCode}");

            _logger.LogDebug($"Completed cache clear");
            return invalidateCacheResponseCode;
        }
    }
}
