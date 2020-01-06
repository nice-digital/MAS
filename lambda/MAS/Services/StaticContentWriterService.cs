﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ViewRenderer> _logger;
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ViewRenderer(ILogger<ViewRenderer> logger, ICompositeViewEngine compositeViewEngine, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _compositeViewEngine = compositeViewEngine;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartial = false)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = GetViewEngineResult(controller, viewName, isPartial);

                if (viewResult.Success == false)
                    _logger.LogError($"A view with the name {viewName} could not be found");

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

        private ViewEngineResult GetViewEngineResult(Controller controller, string viewName, bool isPartial)
        {
            if (viewName.StartsWith("~/"))
                return _compositeViewEngine.GetView(_hostingEnvironment.WebRootPath, viewName, !isPartial);
            else
                return _compositeViewEngine.FindView(controller.ControllerContext, viewName, !isPartial);
        }
    }
}
