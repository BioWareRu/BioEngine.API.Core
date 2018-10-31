using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class SitesController : ResponseRequestRestController<Site, int, Entities.Site>
    {
        public SitesController(BaseControllerContext<Site, int> context) : base(context)
        {
        }
    }
}