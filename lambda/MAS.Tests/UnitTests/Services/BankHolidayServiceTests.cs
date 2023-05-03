using MAS.Services;
using MAS.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests
{
    public class BankHolidayServiceTests
    {
        [Theory()]
        [InlineData(true, 25)]
        [InlineData(false, 24)]
        public async Task BankHoliday_TestDates(bool expected, int day)
        {

            //Arrange
            var bankHolidayService = new BankHolidayService(Mock.Of<ILogger<BankHolidayService>>(),
                                                            TestAppSettings.BankHoliday.Default);

            //Act
            var result = await bankHolidayService.IsBankHoliday(new DateTime(2023, 12, day));

            //Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public async Task BankHoliday_TestForException()
        {

            //Arrange
            var bankHolidayService = new BankHolidayService(Mock.Of<ILogger<BankHolidayService>>(),
                                                            TestAppSettings.BankHoliday.ErroneousURL);

            //Assert
            await Assert.ThrowsAsync<Exception>(async () => await bankHolidayService.IsBankHoliday(new DateTime(2023, 12, 25)));
        }

    }
}