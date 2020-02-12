using System;
using System.Collections.Generic;

namespace MAS.Models
{
    public class Weekly
    {
        public string Title { get; set; }
        public DateTime SendDate { get; set; }
        public string CommentaryTitle { get; set; }
        public string CommentarySummary { get; set; }
        public string CommentaryBody { get; set; }
        public List<Item> Items { get; set; }
    }
}
