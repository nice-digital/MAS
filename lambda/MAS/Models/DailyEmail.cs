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

        Dictionary<Speciality, List<Item>> ItemsBySpecialities()
        {
            var dict = new Dictionary<Speciality, List<Item>>();
            foreach(var item in Items)
            {
                if(item.Speciality.Length > 1)
                {
                    foreach(var spec in item.Speciality)
                    {
                        if (dict.ContainsKey(spec))
                            dict[spec].Add(item);
                        else
                            dict.Add(spec, new List<Item> { item });     
                    }
                }
                else
                {
                    var spec = item.Speciality.First();
                    if (dict.ContainsKey(spec))
                        dict[spec].Add(item);
                    else
                        dict.Add(spec, new List<Item> { item });
                }
            }
            return dict;
        }

        public string HTML
        {
            get
            {
                var body = new StringBuilder();
                
                foreach (var pair in ItemsBySpecialities())
                {
                    var speciality = pair.Key.Title;
                    body.Append("<strong>" + speciality + "</strong>");

                    foreach(var item in pair.Value)
                    {
                        body.Append("<br>");
                        body.Append(item.Source.Title);
                        body.Append("<br>");
                        body.Append(item.Title);
                        body.Append("<br>");
                        body.Append(item.ShortSummary);
                        body.Append("<br>");
                        body.Append("<a href='https://www.medicinesresources.nhs.uk/" + @item.Slug + "'>SPS Comment</a>");
                    }
                }

                return body.ToString();
            }
        }
    }
}
