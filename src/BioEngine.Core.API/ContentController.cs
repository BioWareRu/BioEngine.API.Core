using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Entities;
using BioEngine.Core.Web;
using Post = BioEngine.Core.Entities.Post;

namespace BioEngine.Core.API
{
    public abstract class
        ContentController<TEntity, TResponse, TRequest> : RequestRestController<TEntity, TResponse, TRequest>
        where TEntity : Post, IEntity
        where TResponse : class, IResponseRestModel<TEntity>
        where TRequest : class, IRequestRestModel<TEntity>
    {
        protected ContentController(BaseControllerContext<TEntity> context) : base(context)
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
