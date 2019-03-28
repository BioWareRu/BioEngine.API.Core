using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.API.Entities;
using BioEngine.Core.Entities;
using BioEngine.Core.Properties;
using Newtonsoft.Json;

namespace BioEngine.Core.API.Models
{
    public abstract class RestModel<TEntity> where TEntity : class, IEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? DatePublished { get; set; }

        [JsonIgnore] public List<PropertiesEntry> Properties { get; set; }
        public List<PropertiesGroup> PropertiesGroups { get; set; }

        protected virtual Task ParseEntityAsync(TEntity entity)
        {
            Id = entity.Id;
            DateAdded = entity.DateAdded;
            DateUpdated = entity.DateUpdated;
            IsPublished = entity.IsPublished;
            DatePublished = entity.DatePublished;
            PropertiesGroups =
                entity.Properties.Select(PropertiesGroup.Create).ToList();
            return Task.CompletedTask;
        }

        protected virtual Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = entity ?? CreateEntity();
            entity.Id = Id;
            entity.Properties = PropertiesGroups?.Select(s => s.GetPropertiesEntry()).ToList();
            return Task.FromResult(entity);
        }

        protected virtual TEntity CreateEntity()
        {
            return Activator.CreateInstance<TEntity>();
        }
    }
}
