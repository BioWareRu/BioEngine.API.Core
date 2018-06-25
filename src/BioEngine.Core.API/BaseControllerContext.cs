using BioEngine.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.API
{
    public abstract class BaseControllerContext
    {
        public readonly IStorage Storage;
        public ILogger Logger { get; }

        public BaseControllerContext(ILogger logger, IStorage storage)
        {
            Storage = storage;
            Logger = logger;
        }
    }

    public class BaseControllerContext<T> : BaseControllerContext where T : BaseController
    {
        public BaseControllerContext(ILogger<T> logger, IStorage storage) : base(logger, storage)
        {
        }
    }
}