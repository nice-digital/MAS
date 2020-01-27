using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using MAS.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface ICloudFrontService
    {
        Task<CreateInvalidationResponse> InvalidateCacheAsync(List<string> paths);
    }

    public class CloudFrontService : ICloudFrontService
    {
        private readonly AWSConfig _awsConfig;
        private IAmazonCloudFront _cloudFronService;

        public CloudFrontService(AWSConfig awsConfig, IAmazonCloudFront cloudFronService )
        {
            _awsConfig = awsConfig;
            _cloudFronService = cloudFronService;
        }

        public async Task<CreateInvalidationResponse> InvalidateCacheAsync(List<string> paths)
        {
            var invalidationBatch = new InvalidationBatch();
            invalidationBatch.Paths.Items = paths;

            var req = new CreateInvalidationRequest(_awsConfig.DistributionId, invalidationBatch);
            return await _cloudFronService.CreateInvalidationAsync(req);
        }
    }
}
