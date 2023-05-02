namespace MAS.Configuration
{
    public class BankHolidayConfig
    {
        public static BankHolidayConfig Current { get; private set; }

        public BankHolidayConfig()
        {
            Current = this;
        }

        public string SourceURL { get; set; }

    }
}
