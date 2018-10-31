using System.Threading.Tasks;
using BioEngine.Core.API.Models;

namespace BioEngine.Core.API.Entities
{
    public class Page : SiteEntityRestModel<Core.Entities.Page, int>, IRequestRestModel<Core.Entities.Page, int>,
        IResponseRestModel<Core.Entities.Page, int>
    {
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public string Text { get; set; }

        public async Task<Core.Entities.Page> GetEntityAsync(Core.Entities.Page entity)
        {
            entity = await FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            entity.Text = Text;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Page entity)
        {
            await ParseEntityAsync(entity);
            Title = entity.Title;
            Url = entity.Url;
            Text = entity.Text;
        }
    }
}