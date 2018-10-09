using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Response;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("v1/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IEnumerable<ISettingsOptionsResolver> _resolvers;

        public SettingsController(IEnumerable<ISettingsOptionsResolver> resolvers = default)
        {
            _resolvers = resolvers;
        }

        [HttpGet]
        public async Task<ActionResult<ListResponse<SettingsOption>>> Get(
            string settingsKey, string propertyKey)
        {
            var settings = SettingsProvider.GetInstance(settingsKey.Replace("-", "."));
            if (settings == null)
            {
                return NotFound();
            }

            var resolver = _resolvers.FirstOrDefault(r => r.CanResolve(settings));
            if (resolver == null)
            {
                return NotFound();
            }

            var options = await resolver.Resolve(settings, propertyKey.Replace("-", "."));

            return new ListResponse<SettingsOption>(options, options.Count);
        }
    }
}