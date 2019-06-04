using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class TagsController : ResponseRequestRestController<Tag, QueryContext<Tag>, TagsRepository, Entities.Tag>
    {
        public TagsController(BaseControllerContext<Tag, QueryContext<Tag>, TagsRepository> context) : base(context)
        {
        }
    }
}
