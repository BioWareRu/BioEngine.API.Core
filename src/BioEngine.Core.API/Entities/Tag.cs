using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Tag : RestModel<Core.Entities.Tag>, IRequestRestModel<Core.Entities.Tag>,
        IResponseRestModel<Core.Entities.Tag>
    {
        public string Name { get; set; }

        public async Task<Core.Entities.Tag> GetEntityAsync(Core.Entities.Tag entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Name = Name;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Tag entity)
        {
            await ParseEntityAsync(entity);
            Name = entity.Name;
        }
    }
}
