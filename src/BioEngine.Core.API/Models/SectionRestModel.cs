using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Entities;

namespace BioEngine.Core.API.Models
{
    public abstract class SectionRestModel<TEntity> : SiteEntityRestModel<TEntity>,
        IContentRequestRestModel<TEntity>
        where TEntity : Section, ISiteEntity, IEntity
    {
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public StorageItem Logo { get; set; }
        public StorageItem LogoSmall { get; set; }
        public List<Entities.ContentBlock> Blocks { get; set; }
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
            entity.Hashtag = Hashtag;
            return entity;
        }
    }

    public abstract class SectionRestModel<TEntity, TData> : SectionRestModel<TEntity>
        where TEntity : Section, ITypedEntity<TData>, ISiteEntity, IEntity
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

    public abstract class ResponseSectionRestModel<TEntity> : SectionRestModel<TEntity>,
        IResponseRestModel<TEntity>
        where TEntity : Section, ISiteEntity, IEntity
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
            Blocks = entity.Blocks?.Select(Entities.ContentBlock.Create).ToList();
            Hashtag = entity.Hashtag;
            if (entity is ITypedEntity typedEntity)
            {
                TypeTitle = typedEntity.TypeTitle;
            }
        }
    }

    public abstract class ResponseSectionRestModel<TEntity, TData> : SectionRestModel<TEntity>,
        IContentResponseRestModel<TEntity>
        where TEntity : Section, ISiteEntity, IEntity, ITypedEntity<TData>
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
            Blocks = entity.Blocks?.Select(Entities.ContentBlock.Create).ToList();
            Hashtag = entity.Hashtag;
            TypeTitle = entity.TypeTitle;

            Data = entity.Data;
        }
    }
}
