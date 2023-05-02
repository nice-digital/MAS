using Amazon.Runtime;
using MAS.Configuration;
using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace MAS.Tests.UnitTests
{
    public class BankHolidayServiceTests
    {
        [Theory()]
        [InlineData(true, 25)]
        [InlineData(false, 24)]
        public async Task BankHoliday(bool expected, int day)
        {

            //Arrange
            var bankHolidayService = new BankHolidayService(Mock.Of<ILogger<BankHolidayService>>(),
                                                            TestAppSettings.BankHoliday.Default);

            //Act
            var result = await bankHolidayService.IsBankHoliday(new DateTime(2023, 12, day));

            //Assert
            result.ShouldBe(expected);
        }

    }
}