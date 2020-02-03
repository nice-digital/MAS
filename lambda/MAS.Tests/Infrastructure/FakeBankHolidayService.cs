using MAS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Tests.Infrastructure
{
    public class FakeBankHolidayService : IBankHolidayService
    {
        public bool IsBankHoliday(DateTime date)
        {
            if (date == new DateTime(2019, 8, 26))
                return true;

            return false;
        }
    }
}
