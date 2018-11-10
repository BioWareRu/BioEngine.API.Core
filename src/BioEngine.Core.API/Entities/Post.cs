using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Entities
{
    public class Post : SectionEntityRestModel<Core.Entities.Post, int>,
        IResponseRestModel<Core.Entities.Post, int>,
        IRequestRestModel<Core.Entities.Post, int>
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<ContentBlock> Blocks { get; set; }
        public IUser Author { get; set; }
        public int AuthorId { get; set; }
        public bool IsPinned { get; set; }

        protected override async Task ParseEntityAsync(Core.Entities.Post entity)
        {
            await base.ParseEntityAsync(entity);
            Title = entity.Title;
            Url = entity.Url;
            Blocks = entity.Blocks != null
                ? entity.Blocks.Select(ContentBlock.Create).ToList()
                : new List<ContentBlock>();
            AuthorId = entity.AuthorId;
            Author = entity.Author;
            IsPinned = entity.IsPinned;
        }

        protected override async Task<Core.Entities.Post> FillEntityAsync(Core.Entities.Post entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.Title = Title;
            entity.Url = Url;
            return entity;
        }

        public async Task SetEntityAsync(Core.Entities.Post entity)
        {
            await ParseEntityAsync(entity);
        }

        public async Task<Core.Entities.Post> GetEntityAsync(Core.Entities.Post entity)
        {
            return await FillEntityAsync(entity);
        }
    }

    public class ContentBlock
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string TypeTitle { get; set; }
        public object Data { get; set; }

        public static ContentBlock Create(Core.Entities.PostBlock block)
        {
            var contentBlock = new ContentBlock
            {
                Id = block.Id,
                Type = block.Type,
                TypeTitle = block.TypeTitle,
                Data = block.GetData()
            };

            return contentBlock;
        }
    }
}