using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class ContentController : ContentController<ContentItem, int, Entities.ContentItem>
    {
        public ContentController(BaseControllerContext<ContentItem, int> context) : base(context)
        {
        }
    }
}