using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MAS.TagHelpers
{
    /// <summary>
    /// Replace HTML entities with their numeric equivalent e.g. &nbsp; becomes &#x00A0;
    /// </summary>
    public class EntitiesTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = output.Content.IsModified ? output.Content.GetContent() : (await output.GetChildContentAsync()).GetContent();
            content = ReplaceHtmlEntitiesWithUnicode(content);

            output.TagName = null;
            output.Content.SetHtmlContent(content);
        }

        /// <summary>
        /// Replaces HTML entities with their unicode equivalent e.g. &Agrave; becomes &#x00C0; and &nbsp; becomes &#x00A0;
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ReplaceHtmlEntitiesWithUnicode(string html)
        {
            var replacements = new Dictionary<string, string>();
            var regex = new Regex("(&[a-z]{2,7};)"); // E.g. &lt; or &amp; or &eacute; or &epsilon;

            foreach (Match match in regex.Matches(html))
            {
                if (!replacements.ContainsKey(match.Value))
                {
                    var unicode = HttpUtility.HtmlDecode(match.Value);
                    if (unicode.Length == 1)
                    {
                        replacements.Add(match.Value, string.Concat("&#", Convert.ToInt32(unicode[0]), ";"));
                    }
                }
            }

            foreach (var replacement in replacements)
            {
                html = html.Replace(replacement.Key, replacement.Value);
            }
            return html;
        }
    }
}
