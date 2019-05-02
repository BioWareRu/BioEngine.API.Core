using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Users;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API.Controllers
{
    public class PostsController : ContentEntityController<Post, Entities.Post, Entities.PostRequestItem>
    {
        private readonly IUserDataProvider _userDataProvider;

        public PostsController(BaseControllerContext<Post> context, BioEntityMetadataManager metadataManager,
            ContentBlocksRepository blocksRepository, IUserDataProvider userDataProvider) : base(context,
            metadataManager, blocksRepository)
        {
            _userDataProvider = userDataProvider;
        }

        protected override async Task<Post> MapDomainModelAsync(Entities.PostRequestItem restModel,
            Post domainModel = null)
        {
            domainModel = await base.MapDomainModelAsync(restModel, domainModel);
            if (domainModel.AuthorId == 0)
            {
                domainModel.AuthorId = CurrentUser.Id;
            }

            return domainModel;
        }

        public override async Task<ActionResult<StorageItem>> UploadAsync(string name)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name,
                $"posts/{DateTimeOffset.UtcNow.Year.ToString()}/{DateTimeOffset.UtcNow.Month.ToString()}");
        }

        [HttpGet("{postId}/versions")]
        public async Task<ActionResult<List<PostVersionInfo>>> GetVersionsAsync(Guid postId)
        {
            if (Repository is PostsRepository repository)
            {
                var versions = await repository.GetVersionsAsync(postId);
                var userIds =
                    await _userDataProvider.GetDataAsync(versions.Select(v => v.ChangeAuthorId).Distinct().ToArray());
                return Ok(versions.Select(v =>
                        new PostVersionInfo(v.Id, v.DateAdded, userIds.FirstOrDefault(u => u.Id == v.ChangeAuthorId)))
                    .ToList());
            }

            return BadRequest();
        }

        [HttpGet("{postId}/versions/{versionId}")]
        public async Task<ActionResult<Entities.Post>> GetVersionsAsync(Guid postId, Guid versionId)
        {
            if (Repository is PostsRepository repository)
            {
                var version = await repository.GetVersionAsync(postId, versionId);
                if (version == null)
                {
                    return NotFound();
                }

                var post = version.GetPost();
                return Ok(await MapRestModelAsync(post));
            }

            return BadRequest();
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
