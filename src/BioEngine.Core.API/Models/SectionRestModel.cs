using System.Threading.Tasks;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Storage;

namespace BioEngine.Core.API.Models
{
    public abstract class SectionRestModel<TEntity, TEntityPk> : SiteEntityRestModel<TEntity, TEntityPk>,
        IRequestRestModel<TEntity, TEntityPk>
        where TEntity : Section, ISiteEntity<TEntityPk>, IEntity<TEntityPk>
    {
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public StorageItem Logo { get; set; }
        public StorageItem LogoSmall { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual string Hashtag { get; set; }

        public async Task<TEntity> GetEntityAsync(TEntity entity)
        {
            return await FillEntityAsync(entity);
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            entity.Logo = Logo;
            entity.LogoSmall = LogoSmall;
            entity.ShortDescription = ShortDescription;
            entity.Hashtag = Hashtag;
            return entity;
        }
    }

    public abstract class SectionRestModel<TEntity, TEntityPk, TData> : SectionRestModel<TEntity, TEntityPk>
        where TEntity : Section, ITypedEntity<TData>, ISiteEntity<TEntityPk>, IEntity<TEntityPk>
        where TData : TypedData, new()
    {
        public TData Data { get; set; }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.Data = Data;
            return entity;
        }
    }

    public abstract class ResponseSectionRestModel<TEntity, TEntityPk> : SectionRestModel<TEntity, TEntityPk>,
        IResponseRestModel<TEntity, TEntityPk>
        where TEntity : Section, ISiteEntity<TEntityPk>, IEntity<TEntityPk>
    {
        public virtual string Type { get; set; }
        public string TypeTitle { get; set; }

        public async Task SetEntityAsync(TEntity entity)
        {
            await ParseEntityAsync(entity);
        }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            Type = entity.Type;
            Title = entity.Title;
            Url = entity.Url;
            Logo = entity.Logo;
            LogoSmall = entity.LogoSmall;
            ShortDescription = entity.ShortDescription;
            Hashtag = entity.Hashtag;
            if (entity is ITypedEntity typedEntity)
            {
                TypeTitle = typedEntity.TypeTitle;
            }
        }
    }

    public abstract class ResponseSectionRestModel<TEntity, TEntityPk, TData> : SectionRestModel<TEntity, TEntityPk>,
        IResponseRestModel<TEntity, TEntityPk>
        where TEntity : Section, ISiteEntity<TEntityPk>, IEntity<TEntityPk>, ITypedEntity<TData>
        where TData : TypedData, new()
    {
        public virtual string Type { get; set; }
        public string TypeTitle { get; set; }
        public TData Data { get; set; }

        public async Task SetEntityAsync(TEntity entity)
        {
            await ParseEntityAsync(entity);
        }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            Type = entity.Type;
            Title = entity.Title;
            Url = entity.Url;
            Logo = entity.Logo;
            LogoSmall = entity.LogoSmall;
            ShortDescription = entity.ShortDescription;
            Hashtag = entity.Hashtag;
            TypeTitle = entity.TypeTitle;

            Data = entity.Data;
        }
    }
}
