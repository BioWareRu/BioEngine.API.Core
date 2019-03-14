using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class TagsController : ResponseRequestRestController<Tag, Entities.Tag>
    {
        public TagsController(BaseControllerContext<Tag> context) : base(context)
        {
        }
    }
}
