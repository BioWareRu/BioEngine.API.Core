using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public interface IRequestRestModel<TEntity>
        where TEntity : class, IEntity
    {
        Task<TEntity> GetEntityAsync(TEntity entity);
    }
}
