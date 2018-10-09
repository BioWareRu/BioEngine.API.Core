using System.IO;
using System.Threading.Tasks;
using BioEngine.Core.API.Request;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class SectionController<TRestModel, T, TId, TData> : SectionController<TRestModel, T, TId>
        where TRestModel : SectionRestModel<T, TId, TData>
        where T : Section<TData>, IEntity<TId>
        where TData : TypedData, new()
    {
        protected SectionController(BaseControllerContext context) : base(context)
        {
        }

        protected override async Task<TRestModel> MapRestModel(T domainModel)
        {
            var restModel = await base.MapRestModel(domainModel);
            restModel.Data = domainModel.Data;
            return restModel;
        }

        protected override async Task<T> MapDomainModel(TRestModel restModel, T domainModel = default(T))
        {
            domainModel = await base.MapDomainModel(restModel, domainModel);
            domainModel.Data = restModel.Data;
            return domainModel;
        }
    }

    public abstract class SectionController<TRestModel, T, TId> : RestController<TRestModel, T, TId>
        where T : Section, IEntity<TId>
        where TRestModel : SectionRestModel<T, TId>
    {
        protected SectionController(BaseControllerContext context) : base(context)
        {
        }

        public override async Task<ActionResult<StorageItem>> Upload([FromQuery] string name)
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                return await Storage.SaveFile(ms.GetBuffer(), name,
                    $"sections/{GetUploadPath()}");
            }
        }

        protected abstract string GetUploadPath();

        protected override async Task<TRestModel> MapRestModel(T domainModel)
        {
            var restModel = await base.MapRestModel(domainModel);
            restModel.Title = domainModel.Title;
            restModel.Type = domainModel.Type;
            restModel.Url = domainModel.Url;
            restModel.Logo = domainModel.Logo;
            restModel.LogoSmall = domainModel.LogoSmall;
            restModel.ShortDescription = domainModel.ShortDescription;
            restModel.Hashtag = domainModel.Hashtag;
            restModel.SiteIds = domainModel.SiteIds;
            if (domainModel is ITypedEntity typedEntity)
            {
                restModel.TypeTitle = typedEntity.TypeTitle;
            }

            return restModel;
        }

        protected override async Task<T> MapDomainModel(TRestModel restModel, T domainModel = default(T))
        {
            domainModel = await base.MapDomainModel(restModel, domainModel);
            domainModel.Title = restModel.Title;
            domainModel.Url = restModel.Url;
            domainModel.Logo = restModel.Logo;
            domainModel.LogoSmall = restModel.LogoSmall;
            domainModel.ShortDescription = restModel.ShortDescription;
            domainModel.Hashtag = restModel.Hashtag;
            domainModel.SiteIds = restModel.SiteIds;
            return domainModel;
        }
    }
}