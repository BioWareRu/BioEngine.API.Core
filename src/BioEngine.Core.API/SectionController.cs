using System.IO;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class
        SectionController<TEntity, TData, TRepository, TResponse, TRequest> : ContentEntityController<
            TEntity, TRepository,
            TResponse
            , TRequest>
        where TEntity : Section<TData>, IEntity
        where TData : ITypedData, new()
        where TResponse : class, IContentResponseRestModel<TEntity>
        where TRequest : SectionRestModel<TEntity>, IContentRequestRestModel<TEntity>
        where TRepository : IContentEntityRepository<TEntity, ContentEntityQueryContext<TEntity>>
    {
        protected SectionController(BaseControllerContext<TEntity, ContentEntityQueryContext<TEntity>, TRepository> context,
            BioEntityMetadataManager metadataManager,
            ContentBlocksRepository blocksRepository) : base(context, metadataManager, blocksRepository)
        {
        }


        public override async Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name, Path.Combine("sections", GetUploadPath()));
        }

        protected abstract string GetUploadPath();
    }

    public abstract class
        SectionController<TEntity, TQueryContext, TRepository, TResponse> : ResponseRestController<TEntity,
            TQueryContext, TRepository, TResponse>
        where TEntity : Section, IEntity
        where TResponse : IResponseRestModel<TEntity>
        where TRepository : IBioRepository<TEntity, TQueryContext>
        where TQueryContext : QueryContext<TEntity>, new()
    {
        protected SectionController(BaseControllerContext<TEntity, TQueryContext, TRepository> context) : base(context)
        {
        }
    }
}
