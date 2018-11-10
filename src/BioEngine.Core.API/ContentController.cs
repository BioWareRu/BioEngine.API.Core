using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Web;
using Post = BioEngine.Core.Entities.Post;

namespace BioEngine.Core.API
{
    public abstract class
        ContentController<TEntity, TEntityPk, TResponse, TRequest> : RequestRestController<TEntity, TEntityPk, TResponse
            , TRequest>
        where TEntity : Post, IEntity<TEntityPk>
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