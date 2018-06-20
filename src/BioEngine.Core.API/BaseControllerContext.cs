using Microsoft.Extensions.Logging;

namespace BioEngine.Core.API
{
    public abstract class BaseControllerContext
    {
        public ILogger Logger { get; }

        public BaseControllerContext(ILogger logger)
        {
            Logger = logger;
        }
    }

    public class BaseControllerContext<T> : BaseControllerContext where T : BaseController
    {
        public BaseControllerContext(ILogger<T> logger) : base(logger)
        {
        }
    }
}