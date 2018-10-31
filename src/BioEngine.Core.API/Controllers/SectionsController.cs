using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class SectionsController : SectionController<Section, int, Entities.Section>
    {
        public SectionsController(BaseControllerContext<Section, int> context) : base(context)
        {
        }
    }
}