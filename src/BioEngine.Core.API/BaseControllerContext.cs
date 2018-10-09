using BioEngine.Core.Interfaces;
using BioEngine.Core.Providers;
using Microsoft.Extensions.Logging;

namespace BioEngine.Core.API
{
    public abstract class BaseControllerContext
    {
        public readonly IStorage Storage;
        public readonly SettingsProvider SettingsProvider;
        public ILogger Logger { get; }

        public BaseControllerContext(ILogger logger, IStorage storage, SettingsProvider settingsProvider)
        {
            Storage = storage;
            SettingsProvider = settingsProvider;
            Logger = logger;
        }
    }

    public class BaseControllerContext<T> : BaseControllerContext where T : BaseController
    {
        public BaseControllerContext(ILogger<T> logger, IStorage storage, SettingsProvider settingsProvider) : base(logger,
            storage,
            settingsProvider)
        {
        }
    }
}