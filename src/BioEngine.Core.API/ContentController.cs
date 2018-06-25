using BioEngine.Core.Entities;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API
{
    public abstract class ContentController<T, TId> : RestController<T, TId> where T : ContentItem, IEntity<TId>
    {
        protected ContentController(BaseControllerContext context) : base(context)
        {
        }

        protected T MapContentData(T entity, T newData)
        {
            if (entity.AuthorId == 0)
            {
                entity.AuthorId = CurrentUser.Id;
            }

            entity.Title = newData.Title;
            entity.Url = newData.Url;
            entity.Description = newData.Description;
            entity.SectionIds = newData.SectionIds;

            return entity;
        }
    }
}