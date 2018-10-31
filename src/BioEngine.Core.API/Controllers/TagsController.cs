using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class TagsController : ResponseRequestRestController<Tag, int, Entities.Tag>
    {
        public TagsController(BaseControllerContext<Tag, int> context) : base(context)
        {
        }
    }
}