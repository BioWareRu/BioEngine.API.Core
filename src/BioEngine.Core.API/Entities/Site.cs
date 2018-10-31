using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Site : RestModel<Core.Entities.Site, int>, IRequestRestModel<Core.Entities.Site, int>,
        IResponseRestModel<Core.Entities.Site, int>
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public async Task<Core.Entities.Site> GetEntityAsync(Core.Entities.Site entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Site entity)
        {
            await ParseEntityAsync(entity);
            Title = entity.Title;
            Url = entity.Url;
        }
    }
}