using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        #region Constructor

        private static XmlWriterSettings XmlSettings = new XmlWriterSettings { Async = true, Encoding = Encoding.UTF8, Indent = true };

        private readonly IStaticWebsiteService _staticWebsiteService;
        private readonly IViewRenderer _viewRenderer;
        private readonly IContentService _contentService;
        private readonly ILogger<ContentController> _logger;
        private readonly AWSConfig _awsConfig;

        public ContentController(IStaticWebsiteService staticWebsiteService, 
            IViewRenderer viewRenderer, IContentService contentService, 
            ILogger<ContentController> logger,
            AWSConfig awsConfig)
        {
            _staticWebsiteService = staticWebsiteService;
            _viewRenderer = viewRenderer;
            _contentService = contentService;
            _logger = logger;
            _awsConfig = awsConfig;
        }

        #endregion

        //PUT api/content/
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Item item)
        {
            _logger.LogDebug("Executing PutAsync");
            var month = new DateTime().ToString("yyyy-MM");

            //var getAllItemsTask = _contentService.GetAllItemsAsync();
            var getMonthsItemsTask = _contentService.GetMonthsItemsAsync(month);
            var renderItemHtmlTask = _viewRenderer.RenderViewAsync(this, "~/Views/ContentView.cshtml", item, false);

            var sitemapIndexCreateTask = CreateSiteMapIndex(month);
            //var sitemapXmlCreateTask = CreateSitemapXml(await getAllItemsTask);
            var sitemapXmlCreateTask = CreateSitemapXml(await getMonthsItemsTask);

            // Generate the HTML/XML in parallel
            var sitemapIndexStream = await sitemapIndexCreateTask;
            var sitemapXmlStream = await sitemapXmlCreateTask;
            var itemHtmlString = await renderItemHtmlTask;
            var itemXmlStream = await SerializeItemToXml(item);

            try
            {
                // Write the HTML/XML to S3 in parallel
                var writeContentResult = await _staticWebsiteService.WriteFilesAsync(
                        new StaticContentRequest { FilePath = "sitemapindex.xml", ContentStream = sitemapIndexStream },
                        new StaticContentRequest { FilePath = month + "sitemap.xml", ContentStream = sitemapXmlStream },
                        new StaticContentRequest { FilePath = item.Slug + ".html", ContentBody = itemHtmlString },
                        new StaticContentRequest { FilePath = item.Slug + ".xml", ContentStream = itemXmlStream });

                return Validate(writeContentResult, _logger);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to write item content to the static file store: {e.Message}");
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message, Instance = Request.Path });
            }
        }

        //PUT api/content/initialsitemapindexsetup/
        [HttpPut("initialsitemapindexsetup")]
        public async Task<IActionResult> PutAsync()
        {
            _logger.LogDebug("Executing SiteMapIndex Setup");
            var month = new DateTime(2020 - 01);
            var currentMonth = new DateTime();

            while (month < currentMonth)
            {
                var getMonthsItemsTask = _contentService.GetMonthsItemsAsync(month.ToString());
                var sitemapIndexCreateTask = CreateSiteMapIndex(month.ToString());
                var sitemapXmlCreateTask = CreateSitemapXml(await getMonthsItemsTask);

                // Generate the HTML/XML in parallel
                var sitemapIndexStream = await sitemapIndexCreateTask;
                var sitemapXmlStream = await sitemapXmlCreateTask;

                try
                {
                    // Write the HTML/XML to S3 in parallel
                    var writeContentResult = await _staticWebsiteService.WriteFilesAsync(
                            new StaticContentRequest { FilePath = "sitemapindex.xml", ContentStream = sitemapIndexStream },
                            new StaticContentRequest { FilePath = month + "sitemap.xml", ContentStream = sitemapXmlStream });

                    _logger.LogDebug(Validate(writeContentResult, _logger).ToString());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to write item content to the static file store: {e.Message}");
                    return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message, Instance = Request.Path });
                }

                month.AddMonths(1);
            }

            return Ok();
        }

        private Task<Stream> SerializeItemToXml(Item item)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Item));
            var memoryStream = new MemoryStream();

            return Task.Run<Stream>(() => {
                using (var xmlWriter = XmlWriter.Create(memoryStream, XmlSettings))
                {
                    serializer.Serialize(xmlWriter, item);
                }
                return memoryStream;
            });
        }

        public async Task<Stream> CreateSiteMapIndex(string month)
        {
            string siteMapIndexString = _staticWebsiteService.GetFile("sitemapindex.xml");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(siteMapIndexString);
            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("s", "http://www.sitemaps.org/schemas/sitemap/0.9");
            XmlNodeList xnList = xml.SelectNodes("/s:sitemapindex/s:sitemap", manager);
            
            var sitemaps = new List<(string monthSiteMap, string updatedAt)>();

            //Create a list of all the sitemaps currently in the index
            foreach (XmlNode xn in xnList)
            {
                sitemaps.Add((xn["loc"].InnerText, xn["lastmod"].InnerText));
            }

            //Check if current month is in there. If not, add it.
            if(sitemaps.FindAll(s => s.monthSiteMap.Contains(month)).Count == 0)
            {
                sitemaps.Add((_awsConfig.StaticURL + month + "sitemap.xml", DateTime.Now.ToString()));
            }

            var memoryStream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(memoryStream, XmlSettings))
            {
                await xmlWriter.WriteStartDocumentAsync();
                await xmlWriter.WriteStartElementAsync(null, "sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var sitemap in sitemaps)
                {
                    await xmlWriter.WriteStartElementAsync(null, "sitemap", null);
                    await xmlWriter.WriteElementStringAsync(null, "loc", null, sitemap.monthSiteMap);
                    await xmlWriter.WriteElementStringAsync(null, "lastmod", null, sitemap.updatedAt);
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.FlushAsync();
            }
            return memoryStream;
        }

        private async Task<Stream> CreateSitemapXml(IEnumerable<ItemLight> items)
        {
            var memoryStream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(memoryStream, XmlSettings))
            {
                await xmlWriter.WriteStartDocumentAsync();
                await xmlWriter.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var item in items)
                {
                    await xmlWriter.WriteStartElementAsync(null, "url", null);
                    await xmlWriter.WriteElementStringAsync(null, "loc", null, _awsConfig.StaticURL + item.Slug + ".html");
                    await xmlWriter.WriteElementStringAsync(null, "lastmod", null, item.UpdatedAt.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));
                    await xmlWriter.WriteEndElementAsync();
                }

                await xmlWriter.WriteEndElementAsync();
                await xmlWriter.FlushAsync();
            }
            return memoryStream;
        }
    }
}