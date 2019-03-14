using System.Collections.Generic;
using System.Reflection;
using BioEngine.Core.API.Models;
using BioEngine.Core.Modules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BioEngine.Core.API
{
    public class ApiModule : BioEngineModule<ApiModuleConfig>
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment)
        {
            var assembliesList = new List<Assembly>(Config.Assemblies)
                {typeof(ApiModule).Assembly};
            services.Scan(s =>
                s.FromAssemblies(assembliesList).AddClasses(classes =>
                        classes.AssignableToAny(typeof(IResponseRestModel<>), typeof(IRequestRestModel<>)))
                    .AsSelf());
        }
    }

    public class ApiModuleConfig
    {
        public List<Assembly> Assemblies { get; } = new List<Assembly>();
    }
}
