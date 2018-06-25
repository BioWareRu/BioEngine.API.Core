using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BioEngine.Core.API.Request;
using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;
using BioEngine.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class SectionController<T, TId> : RestController<T, TId> where T : Section, IEntity<TId>
    {
        protected SectionController(BaseControllerContext context) : base(context)
        {
        }

        protected T MapSectionData(T entity, T newData)
        {
            entity.ParentId = newData.ParentId;
            entity.ForumId = newData.ForumId;
            entity.Title = newData.Title;
            entity.Url = newData.Url;
            entity.Logo = newData.Logo;
            entity.LogoSmall = newData.LogoSmall;
            entity.Description = newData.Description;
            entity.ShortDescription = newData.ShortDescription;
            entity.Keywords = newData.Keywords;
            entity.Hashtag = newData.Hashtag;
            entity.SiteIds = newData.SiteIds;

            return entity;
        }

        public override async Task<ActionResult<StorageItem>> Upload([FromQuery] string name)
        {
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                return await Storage.SaveFile(ms.GetBuffer(), name,
                    $"sections/{GetUploadPath()}");
            }
        }

        protected abstract string GetUploadPath();
    }
}