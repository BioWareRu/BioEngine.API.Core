using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public interface IRequestRestModel<TEntity, TEntityPk>
        where TEntity : class, IEntity<TEntityPk>
    {
        Task<TEntity> GetEntityAsync(TEntity entity);
    }
}