using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using ContentBlock = BioEngine.Core.API.Entities.ContentBlock;

namespace BioEngine.Core.API.Models
{
    public interface IRequestRestModel<TEntity>
        where TEntity : class, IEntity
    {
        Task<TEntity> GetEntityAsync(TEntity entity);
    }

    public interface IContentRequestRestModel<TEntity> : IRequestRestModel<TEntity>
        where TEntity : class, IEntity, IContentEntity
    {
        List<ContentBlock> Blocks { get; set; }
    }
}
