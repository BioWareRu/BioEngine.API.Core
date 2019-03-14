using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class MenuController : ResponseRequestRestController<Menu, Entities.Menu>
    {
        public MenuController(BaseControllerContext<Menu> context) : base(context)
        {
        }
    }
}
