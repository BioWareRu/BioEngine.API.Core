using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Users;

namespace BioEngine.Core.API.Entities
{
    public class PostRequestItem : SectionEntityRestModel<Core.Entities.Post>,
        IContentRequestRestModel<Core.Entities.Post>
    {
        public List<ContentBlock> Blocks { get; set; }

        public async Task<Core.Entities.Post> GetEntityAsync(Core.Entities.Post entity)
        {
            return await FillEntityAsync(entity);
        }

        protected override async Task<Core.Entities.Post> FillEntityAsync(Core.Entities.Post entity)
        {
            entity = await base.FillEntityAsync(entity);
            return entity;
        }
    }

    public class Post : PostRequestItem, IContentResponseRestModel<Core.Entities.Post>
    {
        public IUser Author { get; set; }
        public int AuthorId { get; set; }
        public bool IsPinned { get; set; }

        protected override async Task ParseEntityAsync(Core.Entities.Post entity)
        {
            await base.ParseEntityAsync(entity);
            Blocks = entity.Blocks != null
                ? entity.Blocks.OrderBy(b => b.Position).Select(ContentBlock.Create).ToList()
                : new List<ContentBlock>();
            AuthorId = entity.AuthorId;
            Author = entity.Author;
            IsPinned = entity.IsPinned;
        }


        public async Task SetEntityAsync(Core.Entities.Post entity)
        {
            await ParseEntityAsync(entity);
        }
    }

    public class ContentBlock
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string TypeTitle { get; set; }
        public int Position { get; set; }
        public object Data { get; set; }

        public static ContentBlock Create(Core.Entities.ContentBlock block)
        {
            var contentBlock = new ContentBlock
            {
                Id = block.Id,
                Type = block.Type,
                TypeTitle = block.TypeTitle,
                Position = block.Position,
                Data = block.GetData()
            };

            return contentBlock;
        }
    }
}
