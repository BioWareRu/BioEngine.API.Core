using System.Threading.Tasks;
using BioEngine.Core.Entities;

namespace BioEngine.Core.API.Models
{
    public interface IResponseRestModel<in TEntity>
        where TEntity : class, IEntity
    {
        Task SetEntityAsync(TEntity entity);
    }
}
