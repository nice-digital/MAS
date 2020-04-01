using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Models
{
    public class StaticContentRequest
    {
        public string ContentBody { get; set; }

        public Stream ContentStream { get; set; }

        public string FilePath { get; set; }
    }
}
