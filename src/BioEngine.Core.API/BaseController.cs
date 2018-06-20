using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.API
{
    [ApiController]
    [Authorize]
    [Route("v1/[controller]")]
    public abstract class BaseController : Controller
    {
        protected ILogger Logger { get; }

        protected BaseController(BaseControllerContext context)
        {
            Logger = context.Logger;
        }
    }
}