using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using System;

namespace MAS.Controllers
{
    public class ControllerBase : Controller
    {
        protected IActionResult Validate<T>(HttpStatusCode httpStatusCode, ILogger<T> logger)
        {
            if (httpStatusCode == HttpStatusCode.OK)
                return Ok();

            logger.LogWarning("An error has occurred. Status code :" + httpStatusCode);

            if (httpStatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (httpStatusCode == HttpStatusCode.NotFound)
                return NotFound();

            return BadRequest();
        }

    }
}
