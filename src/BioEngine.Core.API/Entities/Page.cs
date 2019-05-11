using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Page : SiteEntityRestModel<Core.Entities.Page>, IContentRequestRestModel<Core.Entities.Page>,
        IContentResponseRestModel<Core.Entities.Page>
    {
        public List<ContentBlock> Blocks { get; set; }

        public async Task<Core.Entities.Page> GetEntityAsync(Core.Entities.Page entity)
        {
            entity = await FillEntityAsync(entity);
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Page entity)
        {
            await ParseEntityAsync(entity);
            Blocks = entity.Blocks?.Select(ContentBlock.Create).ToList();
        }
    }
}
