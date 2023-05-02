using Amazon.Util;
using MailChimp.Net.Models;
using MAS.Configuration;
using MAS.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Amazon.Runtime;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Linq;

namespace MAS.Services
{
    public interface IBankHolidayService 
    {
        Task<bool> IsBankHoliday(DateTime date);
    }

    public class BankHolidayService : IBankHolidayService
    {
        private class GreatBritian
        {
            [JsonProperty("england-and-wales")]
            public Country EnglandAndWales { get; set; }
        }

        private class Country
        {
            public List<Event> Events { get; set; }
        }

        private class Event
        {
            public DateTime Date { get; set; }
        }

        private readonly ILogger<BankHolidayService> _logger;
        private readonly BankHolidayConfig _bankHolidayConfig;

        public BankHolidayService()
        {
            
        }

        public BankHolidayService(ILogger<BankHolidayService> logger, BankHolidayConfig bankHolidayConfig)
        {
            _logger = logger;
            _bankHolidayConfig = bankHolidayConfig;
        }

        public async Task<bool> IsBankHoliday(DateTime date)
        {
            using (WebClient client = new WebClient())
            {
                try
                {

                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(_bankHolidayConfig.SourceURL));
                    var greatBritian = JsonConvert.DeserializeObject<GreatBritian>(jsonStr);

                    var bankHolidayDay = greatBritian.EnglandAndWales
                                                     .Events
                                                     .Where(x => x.Date == date)
                                                     .FirstOrDefault();

                    return bankHolidayDay != null;

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to get data from bank holiday service");
                    throw new Exception($"Failed to get data from bank holiday service", e);
                }
            }

        }
    }
}
