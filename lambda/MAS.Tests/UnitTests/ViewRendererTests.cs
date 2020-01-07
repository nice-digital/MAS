using System.Threading.Tasks;
using MAS.Models;
using MAS.Services;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Shouldly;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MAS.Tests.UnitTests
{
    public class ViewRendererTests
    {
        [Fact]
        public async Task RendersViewToString()
        {
            //Arrange
            var ExpectedHTML = "Test";

            var mockView = new Mock<IView>();
            mockView.Setup(x => x.RenderAsync(It.IsAny<ViewContext>())).Callback<ViewContext>(v => v.Writer.Write(ExpectedHTML));

            var mockViewEngine = new Mock<ICompositeViewEngine>();
            mockViewEngine.Setup(e => e.GetView(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                      .Returns(ViewEngineResult.Found("SomeView", mockView.Object));

            var context = new ControllerContext(new ActionContext(Mock.Of<HttpContext>(), new RouteData { }, new ControllerActionDescriptor { }));
            var mockController = new Mock<Controller>();
            mockController.Object.ControllerContext = context;
            mockController.Object.TempData = Mock.Of<ITempDataDictionary>();

            //Act
            var viewRenderer = new ViewRenderer(Mock.Of<ILogger<ViewRenderer>>(), mockViewEngine.Object, Mock.Of<IHostingEnvironment>());
            var result = await viewRenderer.RenderViewAsync(mockController.Object, "~/AMockedView.cshtml", Mock.Of<Item>(), false);

            //Assert
            result.ShouldBe(ExpectedHTML);
        }
    }
}
