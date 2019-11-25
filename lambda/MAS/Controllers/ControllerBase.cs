using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using Amazon.S3.Model;

namespace MAS.Controllers
{
    public class ControllerBase : Controller
    {
        protected IActionResult Validate(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.OK)
                return Ok();

            if (httpStatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (httpStatusCode == HttpStatusCode.NotFound)
                return NotFound();

            return BadRequest();
        }

    }
}