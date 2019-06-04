using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class MenuController : ResponseRequestRestController<Menu, QueryContext<Menu>, MenuRepository, Entities.Menu>
    {
        public MenuController(BaseControllerContext<Menu, QueryContext<Menu>, MenuRepository> context) : base(context)
        {
        }
    }
}
