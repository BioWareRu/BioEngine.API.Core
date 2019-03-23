using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Repository;
using BioEngine.Core.Users;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Core.API.Controllers
{
    public class PostsController : ContentController<Post, Entities.Post, Entities.PostRequestItem>
    {
        private readonly IEnumerable<EntityMetadata> _entityMetadataList;
        private readonly BioContext _dbContext;
        private readonly IUserDataProvider _userDataProvider;

        public PostsController(BaseControllerContext<Post> context, IEnumerable<EntityMetadata> entityMetadataList,
            BioContext dbContext, IUserDataProvider userDataProvider) : base(context)
        {
            _entityMetadataList = entityMetadataList;
            _dbContext = dbContext;
            _userDataProvider = userDataProvider;
        }

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

        protected override async Task<Post> MapDomainModelAsync(Entities.PostRequestItem restModel,
            Post domainModel = null)
        {
            domainModel = await base.MapDomainModelAsync(restModel, domainModel);
            if (domainModel.AuthorId == 0)
            {
                domainModel.AuthorId = CurrentUser.Id;
            }

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

        public override async Task<ActionResult<StorageItem>> UploadAsync(string name)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name,
                $"posts/{DateTimeOffset.UtcNow.Year}/{DateTimeOffset.UtcNow.Month}");
        }

        [HttpGet("{postId}/versions")]
        public async Task<ActionResult<List<PostVersionInfo>>> GetVersionsAsync(Guid postId)
        {
            var versions = await (Repository as PostsRepository).GetVersionsAsync(postId);
            var userIds =
                await _userDataProvider.GetDataAsync(versions.Select(v => v.ChangeAuthorId).Distinct().ToArray());
            return Ok(versions.Select(v =>
                    new PostVersionInfo(v.Id, v.DateAdded, userIds.FirstOrDefault(u => u.Id == v.ChangeAuthorId)))
                .ToList());
        }

        [HttpGet("{postId}/versions/{versionId}")]
        public async Task<ActionResult<Entities.Post>> GetVersionsAsync(Guid postId, Guid versionId)
        {
            var version = await (Repository as PostsRepository).GetVersionAsync(postId, versionId);
            if (version == null)
            {
                return NotFound();
            }

            var post = version.GetPost();
            return Ok(await MapRestModelAsync(post));
        }
    }

    public class PostVersionInfo
    {
        public PostVersionInfo(Guid id, DateTimeOffset date, IUser user)
        {
            Id = id;
            Date = date;
            User = user;
        }

        public Guid Id { get; }
        public DateTimeOffset Date { get; }

        public IUser User { get; }
    }
}
