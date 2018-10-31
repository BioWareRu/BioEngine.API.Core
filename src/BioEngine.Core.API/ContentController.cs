using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;

namespace BioEngine.Core.API
{
    public abstract class
        ContentController<TEntity, TEntityPk, TResponse> : ResponseRestController<TEntity, TEntityPk, TResponse>
        where TEntity : ContentItem, IEntity<TEntityPk> where TResponse : IResponseRestModel<TEntity, TEntityPk>
    {
        protected ContentController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
        }
    }

    public abstract class
        ContentController<TEntity, TEntityPk, TData, TResponse, TRequest> :
            RequestRestController<TEntity, TEntityPk, TResponse, TRequest>
        where TEntity : ContentItem<TData>, IEntity<TEntityPk>
        where TData : TypedData, new()
        where TResponse : class, IResponseRestModel<TEntity, TEntityPk>
        where TRequest : class, IRequestRestModel<TEntity, TEntityPk>
    {
        protected ContentController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
        }


        protected override async Task<TEntity> MapDomainModelAsync(TRequest restModel,
            TEntity domainModel = null)
        {
            domainModel = await base.MapDomainModelAsync(restModel, domainModel);
            if (domainModel.AuthorId == 0)
            {
                domainModel.AuthorId = CurrentUser.Id;
            }

            return domainModel;
        }
    }
}