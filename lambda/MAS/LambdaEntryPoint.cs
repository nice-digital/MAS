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
            _logger.LogCritical("FunctionHandlerAsync");
            _logger.LogCritical("request.QueryStringParameters ", request.QueryStringParameters);
            _logger.LogCritical("request.PathParameters ", request.PathParameters);
            _logger.LogCritical("request.Resource ", request.Resource);
            _logger.LogCritical("request.Body ", request.Body);
            _logger.LogCritical("", request);
            _logger.LogCritical("request.Path", request.Path);
            _logger.LogCritical("request.RequestContext.Path", request.RequestContext.Path);
            _logger.LogCritical("request.RequestContext.ResourcePath", request.RequestContext.ResourcePath);

            //if (request.Resource == "WarmingLambda")
            //{

            //}

            return base.FunctionHandlerAsync(request, lambdaContext);
        }
    }
}
