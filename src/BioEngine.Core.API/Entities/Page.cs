using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Page : SiteEntityRestModel<Core.Entities.Page>, IContentRequestRestModel<Core.Entities.Page>,
        IContentResponseRestModel<Core.Entities.Page>
    {
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public List<ContentBlock> Blocks { get; set; }

        public async Task<Core.Entities.Page> GetEntityAsync(Core.Entities.Page entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Page entity)
        {
            await ParseEntityAsync(entity);
            Title = entity.Title;
            Url = entity.Url;
            Blocks = entity.Blocks?.Select(ContentBlock.Create).ToList();
        }
    }
}
