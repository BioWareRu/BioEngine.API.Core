using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class PagesController : ResponseRequestRestController<Page, Entities.Page>
    {
        public PagesController(BaseControllerContext<Page> context) : base(context)
        {
        }
    }
}
