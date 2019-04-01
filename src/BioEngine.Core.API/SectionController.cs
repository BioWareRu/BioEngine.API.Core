using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Core.API
{
    public abstract class
        SectionController<TEntity, TData, TResponse, TRequest> : RequestRestController<TEntity,
            TResponse
            , TRequest>
        where TEntity : Section<TData>, IEntity
        where TData : TypedData, new()
        where TResponse : class, IResponseRestModel<TEntity>
        where TRequest : SectionRestModel<TEntity>
    {
        private readonly IEnumerable<EntityMetadata> _entityMetadataList;
        private readonly BioContext _dbContext;

        protected SectionController(BaseControllerContext<TEntity> context,
            IEnumerable<EntityMetadata> entityMetadataList,
            BioContext dbContext) : base(context)
        {
            _entityMetadataList = entityMetadataList;
            _dbContext = dbContext;
        }

        public override async Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name, Path.Combine("sections", GetUploadPath()));
        }

        protected abstract string GetUploadPath();

        private ContentBlock CreateBlock(string type)
        {
            var blockType = _entityMetadataList.Where(entityMetadata =>
                    entityMetadata.EntityType.FullName == type &&
                    typeof(ContentBlock).IsAssignableFrom(entityMetadata.EntityType))
                .Select(e => e.EntityType).FirstOrDefault();
            if (blockType != null)
            {
                return Activator.CreateInstance(blockType) as ContentBlock;
            }

            return null;
        }

        protected override async Task<TEntity> MapDomainModelAsync(TRequest restModel,
            TEntity domainModel = null)
        {
            domainModel = await base.MapDomainModelAsync(restModel, domainModel);

            domainModel.Blocks = new List<ContentBlock>();
            foreach (var contentBlock in restModel.Blocks)
            {
                var block = await _dbContext.Blocks.FirstOrDefaultAsync(b =>
                    b.Id == contentBlock.Id && b.ContentId == domainModel.Id);
                if (block == null)
                {
                    block = CreateBlock(contentBlock.Type);
                }

                if (block != null)
                {
                    block.Id = contentBlock.Id;
                    block.ContentId = domainModel.Id;
                    block.Position = contentBlock.Position;
                    block.SetData(contentBlock.Data);
                    domainModel.Blocks.Add(block);
                }
            }

            return domainModel;
        }
    }

    public abstract class
        SectionController<TEntity, TResponse> : ResponseRestController<TEntity, TResponse>
        where TEntity : Section, IEntity where TResponse : IResponseRestModel<TEntity>
    {
        protected SectionController(BaseControllerContext<TEntity> context) : base(context)
        {
        }
    }
}
