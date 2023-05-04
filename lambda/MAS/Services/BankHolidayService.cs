using MAS.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using MAS.Models;

namespace MAS.Services
{
    public interface IBankHolidayService 
    {
        Task<bool> IsBankHoliday(DateTime date);
    }

    public class BankHolidayService : IBankHolidayService
    {

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
                    var greatBritain = JsonConvert.DeserializeObject<GreatBritain>(jsonStr);

                    var bankHolidayDay = greatBritain.EnglandAndWales
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
