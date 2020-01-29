using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using static Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest;

namespace MAS
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// MAS::MAS.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :
        // When using an ELB's Application Load Balancer as the event source change 
        // the base class to Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        public const string DailyEmailResourceName = "daily";
        public const string WeeklyEmailResourceName = "weekly";

        private ILogger<LambdaEntryPoint> _logger;

        protected override void PostCreateWebHost(IWebHost webHost)
        {
            _logger = webHost.Services.GetRequiredService<ILogger<LambdaEntryPoint>>();

            base.PostCreateWebHost(webHost);
        }

        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Startup>();
        }

        public override Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            // Using an input of {"resource": "daily"} for a CloudWatch trigger allows Lambda deserialization
            // to work on the APIGatewayProxyRequest object. But all the other fields are empty so we have to
            // add fields to our own fake API gateway object that maps to the controller api route
            if (request.Resource == DailyEmailResourceName || request.Resource == WeeklyEmailResourceName)
            {
                _logger.LogInformation($"Received schedule event for {request.Resource}");
                SetRequiredFields(request);
            }

            return base.FunctionHandlerAsync(request, lambdaContext);
        }

        /// <summary>
        /// Sets the bare minimum of fields needed to map the APIGatewayProxyRequest into the DotNetCore runtime.
        /// </summary>
        /// <param name="request">Either "daily" or "weekly"</param>
        private void SetRequiredFields(APIGatewayProxyRequest request)
        {
            var path = request.Resource;

            if (path != DailyEmailResourceName && path != WeeklyEmailResourceName)
                throw new ArgumentException($"request.Resource should either be daily or weekly but was {request.Resource}");

            request.Resource = "/{proxy+}";
            request.Path = "/api/mail/" + path;
            request.HttpMethod = "PUT";
            request.Headers = new Dictionary<string, string>();
            request.MultiValueHeaders = new Dictionary<string, IList<string>>();
            request.QueryStringParameters = null;
            request.MultiValueQueryStringParameters = null;
            request.PathParameters = new Dictionary<string, string>() { { "proxy", "api/mail/" + path } };
            request.Body = null;
            request.IsBase64Encoded = false;
            request.RequestContext = new ProxyRequestContext
            {
                ResourcePath = "/{proxy+}",
                HttpMethod = "PUT",
                Path = "/fake/api/mail/" + path,
                DomainName = "not-used.execute-api.eu-west-1.amazonaws.com",
                Stage = "fake",
                ApiId = "not-used"
            };
        }
    }
}
