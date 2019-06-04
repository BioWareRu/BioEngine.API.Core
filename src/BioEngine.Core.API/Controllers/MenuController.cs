using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class MenuController : ResponseRequestRestController<Menu, MenuRepository, Entities.Menu>
    {
        public MenuController(BaseControllerContext<Menu, MenuRepository> context) : base(context)
        {
        }
    }
}
