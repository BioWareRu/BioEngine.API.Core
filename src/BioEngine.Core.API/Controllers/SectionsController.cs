using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class SectionsController : SectionController<Section, Entities.Section>
    {
        public SectionsController(BaseControllerContext<Section> context) : base(context)
        {
        }
    }
}
