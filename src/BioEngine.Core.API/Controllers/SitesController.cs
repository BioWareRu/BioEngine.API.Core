using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class
        SitesController : ResponseRequestRestController<Site, QueryContext<Site>, SitesRepository, Entities.Site>
    {
        public SitesController(BaseControllerContext<Site, QueryContext<Site>, SitesRepository> context) : base(context)
        {
        }
    }
}
