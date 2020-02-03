using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Moq;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class LambdaEntryPointTests
    {
        [Theory]
        [InlineData(LambdaEntryPoint.DailyEmailResourceName)]
        [InlineData(LambdaEntryPoint.WeeklyEmailResourceName)]
        public void CreatesFakeAPIGatewayRequestForEmailScheduleEvent(string emailType)
        {
            LambdaEntryPoint lambdaEntryPoint = new LambdaEntryPoint();

            APIGatewayProxyRequest scheduleEvent = new APIGatewayProxyRequest
            {
                Resource = emailType
            };

            lambdaEntryPoint.FunctionHandlerAsync(scheduleEvent, Mock.Of<ILambdaContext>());

            scheduleEvent.ShouldSatisfyAllConditions(
                () => scheduleEvent.Path.ShouldBe("/api/mail/" + emailType),
                () => scheduleEvent.Resource.ShouldBe("/{proxy+}"),
                () => scheduleEvent.HttpMethod.ShouldBe("PUT"),
                () => scheduleEvent.PathParameters.ShouldBe(new Dictionary<string, string>() { { "proxy", "api/mail/" + emailType } }),
                () => scheduleEvent.RequestContext.ResourcePath.ShouldBe("/{proxy+}"),
                () => scheduleEvent.RequestContext.HttpMethod.ShouldBe("PUT"),
                () => scheduleEvent.RequestContext.Path.ShouldBe("/fake/api/mail/" + emailType)
            );
        }

        [Fact]
        public void UsesExistingAPIGatewayRequestForNonScheduleEvents()
        {
            LambdaEntryPoint lambdaEntryPoint = new LambdaEntryPoint();

            APIGatewayProxyRequest scheduleEvent = new APIGatewayProxyRequest
            {
                Resource = "/{proxy+}",
                Path = "/api/something",
                HttpMethod = "GET"
            };

            lambdaEntryPoint.FunctionHandlerAsync(scheduleEvent, Mock.Of<ILambdaContext>());

            scheduleEvent.ShouldSatisfyAllConditions(
                () => scheduleEvent.Resource.ShouldBe("/{proxy+}"),
                () => scheduleEvent.Path.ShouldBe("/api/something"),
                () => scheduleEvent.HttpMethod.ShouldBe("GET")
            );
        }
    }
}
