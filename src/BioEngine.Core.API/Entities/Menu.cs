using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Entities;

namespace BioEngine.Core.API.Entities
{
    public class Menu : SingleSiteEntityRestModel<Core.Entities.Menu, int>, IRequestRestModel<Core.Entities.Menu, int>,
        IResponseRestModel<Core.Entities.Menu, int>
    {
        public string Title { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();

        public async Task<Core.Entities.Menu> GetEntityAsync(Core.Entities.Menu entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Title = Title;
            entity.Items = Items;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Menu entity)
        {
            await ParseEntityAsync(entity);
            Title = entity.Title;
            Items = entity.Items;
        }
    }
}