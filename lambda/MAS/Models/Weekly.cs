using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Models
{
    public class Weekly
    {
        public string Title { get; set; }
        public DateTime SendDate { get; set; }
        public string CommentaryTitle { get; set; }
        public string CommentarySummary { get; set; }
        public string FullCommentary { get; set; }
    }
}
