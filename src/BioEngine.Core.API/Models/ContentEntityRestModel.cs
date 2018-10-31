using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public abstract class ContentEntityRestModel<TEntity, TEntityPk, TData> : ContentEntityRestModel<TEntity, TEntityPk>
        where TData : TypedData, new()
        where TEntity : class, IContentEntity<TEntityPk>, ISectionEntity<TEntityPk>, ISiteEntity<TEntityPk>,
        IEntity<TEntityPk>, ITypedEntity<TData>
    {
        public TData Data { get; set; }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            Data = entity.Data;
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.Data = Data;
            return entity;
        }
    }

    public class ContentEntityRestModel<TEntity, TEntityPk> : SectionEntityRestModel<TEntity, TEntityPk>,
        IRequestRestModel<TEntity, TEntityPk>
        where TEntity : class, IContentEntity<TEntityPk>, ISectionEntity<TEntityPk>, ISiteEntity<TEntityPk>,
        IEntity<TEntityPk>
    {
        
        public string Title { get; set; }
        public string Url { get; set; }
        

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            
            Title = entity.Title;
            Url = entity.Url;
            
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            return entity;
        }

        public async Task<TEntity> GetEntityAsync(TEntity entity)
        {
            return await FillEntityAsync(entity);
        }
    }
}