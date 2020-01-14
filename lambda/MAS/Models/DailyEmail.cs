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

        public List<IGrouping<string, Item>> GroupedItems
        {
            get
            {
                return Items.GroupBy(x => x.EvidenceType.Title).ToList();
            }
        }


        public string HTML
        {
            get
            {
                var body = new StringBuilder();
                
                foreach (var group in GroupedItems)
                {
                    string specInEvidenceType = String.Join(',', group.SelectMany(x => x.Speciality.Select(y => y.Title)));
                    body.Append("*|INTERESTED:Daily specialities of interest:" + specInEvidenceType + "|*");
                    body.Append("<div class='evidenceType'>");

                    var evidenceType = group.Key;
                    body.Append("<strong>" + evidenceType + "</strong>");

                    foreach (var item in group)
                    {
                        string itemSpecList = String.Join(',', item.Speciality.Select(x => x.Title));
                        body.Append("*|INTERESTED:Daily specialities of interest:" + itemSpecList + "|*");

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

                        body.Append("*|END:INTERESTED|*");
                    }

                    body.Append("</div>");
                    body.Append("*|END:INTERESTED|*");
                }

                return body.ToString();
            }
        }
    }
}
