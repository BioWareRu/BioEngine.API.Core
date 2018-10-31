using System.IO;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Storage;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class
        SectionController<TEntity, TEntityPk, TData, TResponse, TRequest> : RequestRestController<TEntity, TEntityPk, TResponse
            , TRequest>
        where TEntity : Section<TData>, IEntity<TEntityPk>
        where TData : TypedData, new()
        where TResponse : class, IResponseRestModel<TEntity, TEntityPk>
        where TRequest : class, IRequestRestModel<TEntity, TEntityPk>
    {
        protected SectionController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
        }

        public override async Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                return await Storage.SaveFileAsync(ms.GetBuffer(), name,
                    $"sections/{GetUploadPath()}");
            }
        }

        protected abstract string GetUploadPath();
    }

    public abstract class
        SectionController<TEntity, TEntityPk, TResponse> : ResponseRestController<TEntity, TEntityPk, TResponse>
        where TEntity : Section, IEntity<TEntityPk> where TResponse : IResponseRestModel<TEntity, TEntityPk>
    {
        protected SectionController(BaseControllerContext<TEntity, TEntityPk> context) : base(context)
        {
        }
    }
}