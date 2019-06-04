using System;
using System.Threading.Tasks;
using BioEngine.Core.Entities;

namespace BioEngine.Core.API.Models
{
    public class ContentEntityRestModel<TEntity> : SiteEntityRestModel<TEntity> where TEntity : class, IContentEntity
    {
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            IsPublished = entity.IsPublished;
            DatePublished = entity.DatePublished;
        }
    }
}
