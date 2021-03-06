﻿using Nager.Date;
using System;

namespace MAS.Services
{
    public interface IBankHolidayService 
    {
        bool IsBankHoliday(DateTime date);
    }

    public class BankHolidayService : IBankHolidayService
    {
        public bool IsBankHoliday(DateTime date)
        {
            return DateSystem.IsOfficialPublicHolidayByCounty(date, CountryCode.GB, "GB-ENG");
        }
    }
}
