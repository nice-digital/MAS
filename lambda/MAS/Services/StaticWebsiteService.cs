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

        Task<HttpStatusCode> WriteContentAndInvalidateCacheAsync(StaticContentRequest[] requests);
    }
    public class S3StaticWebsiteService : IStaticWebsiteService
    {
        #region Constructor

        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3StaticWebsiteService> _logger;
        private readonly AWSConfig _awsConfig;
        private readonly IAmazonCloudFront _cloudFronService;

        public S3StaticWebsiteService(IAmazonS3 amazonS3, ILogger<S3StaticWebsiteService> logger, AWSConfig awsConfig, IAmazonCloudFront cloudFronService)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
            _awsConfig = awsConfig;
            _cloudFronService = cloudFronService;
        }

        #endregion


        public async Task<HttpStatusCode> WriteContentAndInvalidateCacheAsync(params StaticContentRequest[] requests)
        {
            var taskDict = new Dictionary<StaticContentRequest, Task<PutObjectResponse>>();
            foreach(var req in requests)
                taskDict.Add(req, WriteFileAsync(req));

            //This is basically syncronous 
            foreach(var task in taskDict)
            {
                var responseCode = (await task.Value).HttpStatusCode;
                if (responseCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing {task.Key} resulted in a status code of {responseCode}");
                    return responseCode;
                }
            }

            //This is all pointless and just add unnessesary complexity
            //Should I revert to a format similar to the taskDict foreach?
            var invalidateCacheTasks = new List<Task<HttpStatusCode>>();
            foreach (var task in taskDict)
                invalidateCacheTasks.Add(InvalidateCacheAsync(task.Key.FilePath));

            var failedTask = Task.WhenAll(invalidateCacheTasks.ToArray()).Result.FirstOrDefault(x => x != HttpStatusCode.OK);

            if (failedTask == default(HttpStatusCode))
                return failedTask;
            else
                return HttpStatusCode.OK;
        }
        
        private async Task<PutObjectResponse> WriteFileAsync(StaticContentRequest item)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _awsConfig.BucketName,
                Key = item.FilePath
            };

            if (string.IsNullOrEmpty(request.ContentBody))
                request.InputStream = item.ContentStream;
            else
                request.ContentBody = item.ContentBody;

            return await _amazonS3.PutObjectAsync(request);
        }

        private async Task<HttpStatusCode> InvalidateCacheAsync(string path)
        {
            var invalidationBatch = new InvalidationBatch();
            invalidationBatch.Paths.Items = new List<string> { path };

            var req = new CreateInvalidationRequest(_awsConfig.DistributionId, invalidationBatch);
            var invalidateCacheResponseCode = (await _cloudFronService.CreateInvalidationAsync(req)).HttpStatusCode;

            if (invalidateCacheResponseCode != HttpStatusCode.OK)
                _logger.LogError($"Cache invalidation failed for {path} and resulted in a status code of {invalidateCacheResponseCode}");

            return invalidateCacheResponseCode;
        }
    }
}
