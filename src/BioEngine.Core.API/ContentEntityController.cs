using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;

namespace BioEngine.Core.API
{
    public abstract class
        ContentEntityController<TEntity, TResponse, TRequest> : RequestRestController<TEntity, TResponse, TRequest>
        where TEntity : class, IContentEntity, IEntity
        where TResponse : class, IContentResponseRestModel<TEntity>
        where TRequest : class, IContentRequestRestModel<TEntity>
    {
        private readonly ContentBlocksRepository _blocksRepository;
        protected BioEntityMetadataManager MetadataManager { get; }

        protected ContentEntityController(BaseControllerContext<TEntity> context,
            BioEntityMetadataManager metadataManager, ContentBlocksRepository blocksRepository) : base(context)
        {
            _blocksRepository = blocksRepository;
            MetadataManager = metadataManager;
        }

        private ContentBlock CreateBlock(string type)
        {
            var blockType = MetadataManager.GetBlocksMetadata().Where(entityMetadata =>
                    entityMetadata.Type == type &&
                    typeof(ContentBlock).IsAssignableFrom(entityMetadata.ObjectType))
                .Select(e => e.ObjectType).FirstOrDefault();
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
            var dbBlocks = await _blocksRepository.GetByIdsAsync(restModel.Blocks.Select(b => b.Id).ToArray());
            foreach (var contentBlock in restModel.Blocks)
            {
                var block = dbBlocks.FirstOrDefault(b => b.Id == contentBlock.Id && b.ContentId == domainModel.Id) ??
                            CreateBlock(contentBlock.Type);

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
}
