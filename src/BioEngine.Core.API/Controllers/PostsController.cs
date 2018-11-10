using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Web;

namespace BioEngine.Core.API.Controllers
{
    public class PostsController : ContentController<Post, int, Entities.Post, Entities.Post>
    {
        private readonly IEnumerable<EntityMetadata> _entityMetadataList;

        public PostsController(BaseControllerContext<Post, int> context, IEnumerable<EntityMetadata> entityMetadataList) : base(context)
        {
            _entityMetadataList = entityMetadataList;
        }
        
        private PostBlock CreateBlock(string type)
        {
            var blockType = _entityMetadataList.Where(entityMetadata =>
                    entityMetadata.EntityType.FullName == type &&
                    typeof(PostBlock).IsAssignableFrom(entityMetadata.EntityType))
                .Select(e => e.EntityType).FirstOrDefault();
            if (blockType != null)
            {
                return Activator.CreateInstance(blockType) as PostBlock;
            }


            return null;
        }
        
        protected override async Task<Post> MapDomainModelAsync(Entities.Post restModel,
            Post domainModel = null)
        {
            domainModel = await base.MapDomainModelAsync(restModel, domainModel);
            if (domainModel.AuthorId == 0)
            {
                domainModel.AuthorId = CurrentUser.Id;
            }
            domainModel.Blocks = new List<PostBlock>();
            foreach (var contentBlock in restModel.Blocks)
            {
                var block = CreateBlock(contentBlock.Type);
                if (block != null)
                {
                    block.Id = contentBlock.Id;
                    block.Post = domainModel;
                    block.SetData(contentBlock.Data);
                    domainModel.Blocks.Add(block);
                }
            }
            return domainModel;
        }
    }
}