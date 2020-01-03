using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAS.Models
{
    public class DailyEmail
    {
        public List<Item> Items { get; set; }

        public List<IGrouping<EvidenceType, Item>> GroupedItems
        {
            get
            {
                return Items.GroupBy(x => x.EvidenceType).ToList();
            }
        }


        public string HTML
        {
            get
            {
                var body = new StringBuilder();
                
                foreach (var group in GroupedItems)
                {
                    var evidenceType = group.Single().EvidenceType.Title;

                    body.Append("<div class='evidenceType'>");
                    body.Append("<strong>" + evidenceType + "</strong>");

                    foreach(var item in group)
                    {
                        body.Append("<div class='item'>");
                        body.Append(item.Title);
                        body.Append("<br>");
                        body.Append(item.Source.Title);
                        body.Append("<br>");
                        body.Append(String.Join(" | ", item.Speciality.Select(x => x.Title)));
                        body.Append("<br>");
                        body.Append(item.ShortSummary);
                        body.Append("<br>");
                        body.Append("<a href='https://www.medicinesresources.nhs.uk/" + @item.Slug + "'>SPS Comment</a>");
                        body.Append("</div>");
                    }

                    body.Append("</div>");
                }

                return body.ToString();
            }
        }
    }
}
