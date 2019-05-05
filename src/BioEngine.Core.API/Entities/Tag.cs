using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Tag : RestModel<Core.Entities.Tag>, IRequestRestModel<Core.Entities.Tag>,
        IResponseRestModel<Core.Entities.Tag>
    {
        public string Title { get; set; }

        public async Task<Core.Entities.Tag> GetEntityAsync(Core.Entities.Tag entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Title = Title;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Tag entity)
        {
            await ParseEntityAsync(entity);
            Title = entity.Title;
        }
    }
}
