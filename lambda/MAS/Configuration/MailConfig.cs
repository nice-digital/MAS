using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Configuration
{
    public class MailConfig
    {
        public static MailConfig Current { get; private set; }

        public MailConfig()
        {
            Current = this;
        }

        public string ReplyTo { get; set; }
        public string FromName { get; set; }
        public string DailySubject { get; set; }
    }
}
