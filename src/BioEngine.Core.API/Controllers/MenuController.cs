using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class MenuController : ResponseRequestRestController<Menu, int, Entities.Menu>
    {
        public MenuController(BaseControllerContext<Menu, int> context) : base(context)
        {
        }
    }
}