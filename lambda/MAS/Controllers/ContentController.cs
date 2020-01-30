using MAS.Configuration;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Amazon.CloudFront;

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
            var getAllItemsTask = _contentService.GetAllItemsAsync();
            var renderItemHtmlTask = _viewRenderer.RenderViewAsync(this, "~/Views/ContentView.cshtml", item, false);
            var sitemapXmlCreateTask = CreateSitemapXml(await getAllItemsTask);

            // Generate the HTML/XML in parallel
            var sitemapXmlStream = await sitemapXmlCreateTask;
            var itemHtmlString = await renderItemHtmlTask;
            var itemXmlStream = await SerializeItemToXml(item);

            try
            {
                // Write the HTML/XML to S3 in parallel
                var writeContentResult = await _staticWebsiteService.WriteFilesAsync(
                        new StaticContentRequest { FilePath = "sitemap.xml", ContentStream = sitemapXmlStream },
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