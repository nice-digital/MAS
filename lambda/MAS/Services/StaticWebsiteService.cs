using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IStaticWebsiteService
    {
        /// <summary>
        /// Asynchronous writes a files to the statice website and invalidates cache for those items.
        /// </summary>
        /// <param name="requests">These objects contain the file paths relative to the root e.g. "sitemap.xml" and file data.</param>
        /// <returns>The http status code of the request to write the file</returns>
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
            HttpStatusCode responseCode = HttpStatusCode.OK;
            var taskDict = new Dictionary<StaticContentRequest, Task<PutObjectResponse>>();
            foreach(var req in requests)
                taskDict.Add(req, WriteFileAsync(req));

            foreach(var task in taskDict)
            {
                responseCode = (await task.Value).HttpStatusCode;
                if (responseCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing {task.Key.FilePath} resulted in a status code of {responseCode}");
                    return responseCode;
                }
            }

            if (_cloudFrontConfig.Enabled == true && requests.Count() > 0)
                responseCode = await InvalidateCacheAsync(taskDict.Select(x => "/" + x.Key.FilePath).ToList());

            return responseCode;

        }
        
        private async Task<PutObjectResponse> WriteFileAsync(StaticContentRequest item)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _awsConfig.BucketName,
                Key = item.FilePath
            };

            if (string.IsNullOrEmpty(item.ContentBody))
                request.InputStream = item.ContentStream;
            else
                request.ContentBody = item.ContentBody;

            _logger.LogDebug("TESTING THE LOGS !!!!!!!!!!!!!!!!!!!!!!!!!");
            var response = await _amazonS3.PutObjectAsync(request);

            return response;
        }

        private async Task<HttpStatusCode> InvalidateCacheAsync(List<string> paths)
        {           
            var invalidationBatch = new InvalidationBatch()
            {
                CallerReference = DateTime.Now.Ticks.ToString(),
                Paths = new Paths()
                {
                    Items = paths,
                    Quantity = paths.Count()
                }
            };

            var req = new CreateInvalidationRequest(_cloudFrontConfig.DistributionID, invalidationBatch);
            var invalidateCacheResponse = (await _cloudFrontService.CreateInvalidationAsync(req));
            var invalidateCacheResponseCode = invalidateCacheResponse.HttpStatusCode;
            var requestId = invalidateCacheResponse?.ResponseMetadata?.RequestId;
            var invalidationId = invalidateCacheResponse?.Invalidation?.Id;

            if (invalidateCacheResponseCode != HttpStatusCode.Created)
                _logger.LogError($"Cache invalidation failed.\n Status code: {invalidateCacheResponseCode}\n Request ID: {requestId}\n Invalidation ID: {invalidationId}");

            return invalidateCacheResponseCode;
        }
    }
}
