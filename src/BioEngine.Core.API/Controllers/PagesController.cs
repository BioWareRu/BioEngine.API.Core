using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class PagesController : ResponseRequestRestController<Page, int, Entities.Page>
    {
        public PagesController(BaseControllerContext<Page, int> context) : base(context)
        {
        }
    }
}