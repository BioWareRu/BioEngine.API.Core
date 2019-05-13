using System.Reflection;
using BioEngine.Core.API.Models;
using BioEngine.Core.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BioEngine.Core.API
{
    public class ApiModule : WebModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);
            services.RegisterApiEntities(GetType().Assembly);
        }
    }

    public static class ApiServiceExtensions
    {
        public static IServiceCollection RegisterApiEntities(this IServiceCollection services, Assembly assembly)
        {
            return services.Scan(s =>
                s.FromAssemblies(assembly).AddClasses(classes =>
                        classes.AssignableToAny(typeof(IResponseRestModel<>), typeof(IRequestRestModel<>)))
                    .AsSelf());
        }
    }
}
