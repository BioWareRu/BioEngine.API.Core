using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public interface IResponseRestModel<in TEntity, TEntityPk>
        where TEntity : class, IEntity<TEntityPk>
    {
        Task SetEntityAsync(TEntity entity);
    }
}