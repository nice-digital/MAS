using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IViewRenderer
    {
        Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartial = false);
    }

    public class ViewRenderer : IViewRenderer
    {
        #region Constructor

        private readonly ILogger<ViewRenderer> _logger;
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ViewRenderer(ILogger<ViewRenderer> logger, ICompositeViewEngine compositeViewEngine, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _compositeViewEngine = compositeViewEngine;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion

        public async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartial = false)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = _compositeViewEngine.GetView(_hostingEnvironment.WebRootPath, viewName, !isPartial);

                if (viewResult.Success == false)
                {
                    _logger.LogError($"A view with the name {viewName} could not be found");
                    throw new Exception();
                }

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
