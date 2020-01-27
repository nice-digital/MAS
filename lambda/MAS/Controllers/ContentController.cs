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
        private ICloudFrontService _cloudFrontService;

        public ContentController(IStaticWebsiteService staticWebsiteService, 
            IViewRenderer viewRenderer, IContentService contentService, 
            ILogger<ContentController> logger,
            AWSConfig awsConfig,
            ICloudFrontService cloudFrontService)
        {
            _staticWebsiteService = staticWebsiteService;
            _viewRenderer = viewRenderer;
            _contentService = contentService;
            _logger = logger;
            _awsConfig = awsConfig;
            _cloudFrontService = cloudFrontService;
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
                var sitemapXmlWriteTask = _staticWebsiteService.WriteFileAsync("sitemap.xml", sitemapXmlStream);
                var itemHtmlWriteTask = _staticWebsiteService.WriteFileAsync(item.Slug + ".html", itemHtmlString);
                var itemXmlWriteTask = _staticWebsiteService.WriteFileAsync(item.Slug + ".xml", itemXmlStream);

                var sitemapXmlResponseStatus = await sitemapXmlWriteTask;
                var itemHtmlResponseStatus = await itemHtmlWriteTask;
                var itemXmlResponseStatus = await itemXmlWriteTask;

                if (sitemapXmlResponseStatus != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing sitemap XML resulted in a status code of {sitemapXmlResponseStatus}");
                    return Validate(sitemapXmlResponseStatus, _logger);
                }
                else if (itemHtmlResponseStatus != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing item HTML resulted in a status code of {itemHtmlResponseStatus}");
                    return Validate(itemHtmlResponseStatus, _logger);
                }
                else if (itemXmlResponseStatus != HttpStatusCode.OK)
                {
                    _logger.LogError($"Writing item XML resulted in a status code of {itemXmlResponseStatus}");
                    return Validate(itemXmlResponseStatus, _logger);
                }

                //Cache invalidation
                var paths = new List<string>()
                {
                    _awsConfig.ServiceURL + "/sitemap.xml",
                    _awsConfig.ServiceURL + "/" + item.Slug + ".html",
                    _awsConfig.ServiceURL + "/" + item.Slug + ".xml"
                };

                var invalidateCacheTask = _cloudFrontService.InvalidateCacheAsync(paths);
                var invalidateCacheResponseCode = (await invalidateCacheTask).HttpStatusCode;

                if (invalidateCacheResponseCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"Cache invalidation failed and resulted in a status code of {invalidateCacheResponseCode}");
                    return Validate(invalidateCacheResponseCode, _logger);
                }

                return Validate(HttpStatusCode.OK, _logger);
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