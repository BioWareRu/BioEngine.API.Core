using System.Threading.Tasks;
using BioEngine.Core.API.Request;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;

namespace BioEngine.Core.API
{
    public abstract class ContentController<TRestModel, T, TId> : RestController<TRestModel, T, TId>
        where T : ContentItem, IEntity<TId> where TRestModel : ContentEntityRestModel<TId>
    {
        protected ContentController(BaseControllerContext<T, TId> context) : base(context)
        {
        }

        protected override async Task<TRestModel> MapRestModel(T domainModel)
        {
            var restModel = await base.MapRestModel(domainModel);
            restModel.Type = domainModel.Type;
            restModel.Title = domainModel.Title;
            restModel.Url = domainModel.Url;
            restModel.SectionIds = domainModel.SectionIds;
            restModel.TagIds = domainModel.TagIds;
            if (domainModel is ITypedEntity typedEntity)
            {
                restModel.TypeTitle = typedEntity.TypeTitle;
            }

            return restModel;
        }

        // ReSharper disable once OptionalParameterHierarchyMismatch
        protected override async Task<T> MapDomainModel(TRestModel restModel, T domainModel = default(T))
        {
            domainModel = await base.MapDomainModel(restModel, domainModel);
            if (domainModel.AuthorId == 0)
            {
                domainModel.AuthorId = CurrentUser.Id;
            }

            domainModel.Title = restModel.Title;
            domainModel.Url = restModel.Url;
            domainModel.SectionIds = restModel.SectionIds;
            domainModel.TagIds = restModel.TagIds;
            return domainModel;
        }
    }

    public abstract class ContentController<TRestModel, T, TId, TData> : ContentController<TRestModel, T, TId>
        where TRestModel : ContentEntityRestModel<TId, TData>
        where T : ContentItem<TData>, IEntity<TId>
        where TData : TypedData, new()
    {
        protected ContentController(BaseControllerContext<T, TId> context) : base(context)
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
}