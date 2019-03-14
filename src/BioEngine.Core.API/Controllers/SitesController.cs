using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class SitesController : ResponseRequestRestController<Site, Entities.Site>
    {
        public SitesController(BaseControllerContext<Site> context) : base(context)
        {
        }
    }
}
