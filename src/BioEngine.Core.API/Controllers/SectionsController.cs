using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class SectionsController : SectionController<Section, ContentEntityQueryContext<Section>, SectionsRepository,
        Entities.Section>
    {
        public SectionsController(
            BaseControllerContext<Section, ContentEntityQueryContext<Section>, SectionsRepository> context) :
            base(context)
        {
        }
    }
}
