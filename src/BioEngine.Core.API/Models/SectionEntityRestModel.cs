using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public abstract class SectionEntityRestModel<TEntity, TEntityPk> : SiteEntityRestModel<TEntity, TEntityPk>
        where TEntity : class, ISectionEntity<TEntityPk>, ISiteEntity<TEntityPk>, IEntity<TEntityPk>
    {
        public int[] SectionIds { get; set; }
        public int[] TagIds { get; set; }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            SectionIds = entity.SectionIds;
            TagIds = entity.TagIds;
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.SectionIds = SectionIds;
            entity.TagIds = TagIds;
            return entity;
        }
    }
}