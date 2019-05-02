using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Entities;
using ContentBlock = BioEngine.Core.API.Entities.ContentBlock;

namespace BioEngine.Core.API.Models
{
    public interface IResponseRestModel<in TEntity>
        where TEntity : class, IEntity
    {
        Task SetEntityAsync(TEntity entity);
    }

    public interface IContentResponseRestModel<TEntity> : IResponseRestModel<TEntity>
        where TEntity : class, IEntity, IContentEntity
    {
        List<ContentBlock> Blocks { get; set; }
    }
}
