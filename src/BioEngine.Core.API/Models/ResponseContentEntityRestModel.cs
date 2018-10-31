using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public abstract class
        ResponseContentEntityRestModel<TEntity, TEntityPk, TData> : ContentEntityRestModel<TEntity, TEntityPk, TData>,
            IResponseRestModel<TEntity, TEntityPk>
        where TEntity : ContentItem, IContentEntity<TEntityPk>, ISectionEntity<TEntityPk>, ISiteEntity<TEntityPk>,
        IEntity<TEntityPk>, ITypedEntity<TData>
        where TData : TypedData, new()
    {
        public IUser Author { get; set; }
        public int AuthorId { get; set; }
        public string Type { get; set; }
        public string TypeTitle { get; set; }
        public bool IsPinned { get; set; }

        public async Task SetEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            AuthorId = entity.AuthorId;
            Author = entity.Author;
            Type = entity.Type;
            TypeTitle = entity.TypeTitle;

            IsPinned = entity.IsPinned;
            Data = entity.Data;
        }
    }

    public abstract class
        ResponseContentEntityRestModel<TEntity, TEntityPk> : ContentEntityRestModel<TEntity, TEntityPk>,
            IResponseRestModel<TEntity, TEntityPk>
        where TEntity : ContentItem, IContentEntity<TEntityPk>, ISectionEntity<TEntityPk>, ISiteEntity<TEntityPk>,
        IEntity<TEntityPk>
    {
        public IUser Author { get; set; }
        public int AuthorId { get; set; }
        public string Type { get; set; }
        public string TypeTitle { get; set; }
        public bool IsPinned { get; set; }

        public async Task SetEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            Author = entity.Author;
            AuthorId = entity.AuthorId;
            Type = entity.Type;
            if (entity is ITypedEntity typedEntity)
            {
                TypeTitle = typedEntity.TypeTitle;
            }

            IsPinned = entity.IsPinned;
        }
    }
}